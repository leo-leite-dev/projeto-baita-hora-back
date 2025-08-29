using BaitaHora.Domain.Common.ValueObjects;
using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class Company : Entity
{
    public string CompanyName { get; private set; } = string.Empty;
    public CNPJ Cnpj { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public Phone CompanyPhone { get; private set; }
    public Email CompanyEmail { get; private set; }

    public string? TradeName { get; private set; } = string.Empty;

    public CompanyImage? Image { get; private set; }

    private readonly List<CompanyMember> _members = new();
    public IReadOnlyCollection<CompanyMember> Members => _members.AsReadOnly();

    private readonly List<CompanyPosition> _companyPositions = new();
    public IReadOnlyCollection<CompanyPosition> Positions => _companyPositions.AsReadOnly();

    private readonly List<CompanyServiceOffering> _companyServiceOfferings = new();
    public IReadOnlyCollection<CompanyServiceOffering> ServiceOfferings => _companyServiceOfferings.AsReadOnly();

    private Company() { }

    public static Company Create(string companyName, CNPJ cnpj, Address address, string? tradeName, Phone companyPhone, Email companyEmail)
    {
        if (address is null)
            throw new CompanyException("Endereço é obrigatório.");

        var company = new Company
        {
            CompanyName = companyName.Trim(),
            Cnpj = cnpj,
            Address = address,
            TradeName = ActivatableEntity.NormalizeSpaces(tradeName),
            CompanyPhone = companyPhone,
            CompanyEmail = companyEmail,
        };

        company.ValidateInvariants();
        return company;
    }

    public bool Rename(string newName)
    {
        if (CompanyName.Equals(newName))
            return false;

        CompanyName = newName;
        return true;
    }

    public bool ChangeAddress(Address newAddress)
    {
        if (newAddress is null)
            throw new CompanyException("Endereço inválido.");

        if (Address?.Equals(newAddress) == true)
            return false;

        Address = newAddress;
        return true;
    }

    public bool ChangeTradeName(string? newTradeName)
    {
        var normalized = ActivatableEntity.NormalizeSpaces(newTradeName);

        if (string.Equals(ActivatableEntity.NormalizeSpaces(TradeName), normalized, StringComparison.Ordinal))
            return false;

        TradeName = normalized;
        return true;
    }

    public bool ChangePhone(Phone newPhone)
    {
        if (CompanyPhone.Equals(newPhone))
            return false;

        CompanyPhone = newPhone;
        return true;
    }

    public bool ChangeEmail(Email newEmail)
    {
        if (CompanyEmail.Equals(newEmail))
            return false;

        CompanyEmail = newEmail;
        return true;
    }

    private void ValidateInvariants()
    {

        if (string.IsNullOrWhiteSpace(CompanyName))
            throw new CompanyException("Nome completo é obrigatório.");

        if (CompanyName.Length < 3 || CompanyName.Length > 120)
            throw new UserException("Nome completo deve ter de 3 a 120 caracteres.");

        if (Cnpj == default)
            throw new CompanyException("CNPJ é obrigatório.");

        if (TradeName is { Length: > 120 })
            throw new CompanyException("Nome fantasia deve ter no máximo 120 caracteres.");

        if (CompanyPhone == default)
            throw new CompanyException("Telefone é obrigatório.");

        if (CompanyEmail == default)
            throw new CompanyException("Email é obrigatório.");
    }

    //Users
    public CompanyMember AddOwnerFounder(Guid userId)
    {
        if (_members.Any(m => m.Role == CompanyRole.Owner && m.IsActive))
            throw new CompanyException("Já existe um Owner para esta empresa.");

        var member = CompanyMember.CreateFounder(Id, userId);
        _members.Add(member);

        var founder = EnsurePositionByName(
            positionName: "Fundador",
            accessLevel: CompanyRole.Owner,
            isSystem: true
        );

        member.SetPrimaryPosition(founder);

        return member;
    }

    public CompanyMember AddMemberWithPrimaryPosition(Guid userId, CompanyPosition position)
    {
        if (position is null)
            throw new CompanyException("Posição é obrigatória.");

        if (position.CompanyId != Id)
            throw new CompanyException("Cargo não pertence a esta empresa.");

        if (!position.IsActive)
            throw new CompanyException("Não é possível atribuir um cargo inativo.");

        if (position.AccessLevel == CompanyRole.Owner)
            throw new CompanyException("Para Owner, use o fluxo de fundador (AddOwnerFounder).");

        if (_members.Any(m => m.UserId == userId && m.IsActive))
            throw new CompanyException("Usuário já é membro ativo da empresa.");

        var member = CompanyMember.CreateMember(Id, userId, position.AccessLevel);
        member.SetPrimaryPosition(position);

        _members.Add(member);
        return member;
    }

    public CompanyMember ChangeMemberPrimaryPosition(Guid userId, Guid newPositionId, bool alignRoleToPosition = false)
    {
        var newPosition = _companyPositions.SingleOrDefault(p => p.Id == newPositionId && p.IsActive)
            ?? throw new CompanyException("Cargo não encontrado ou inativo.");

        var member = _members.SingleOrDefault(m => m.UserId == userId && m.IsActive)
            ?? throw new CompanyException("Membro não encontrado ou inativo.");

        member.SetPrimaryPosition(newPosition);

        if (alignRoleToPosition)
            member.ChangeRole(newPosition.AccessLevel);

        return member;
    }

    // ===== Positions =====

    public CompanyPosition AddPosition(string positionName, CompanyRole accessLevel, bool isSystem = false)
    {
        positionName = ActivatableEntity.NormalizeSpaces(positionName);

        if (string.IsNullOrWhiteSpace(positionName))
            throw new CompanyException("Nome do cargo é obrigatório.");

        if (_companyPositions.Any(p =>
                string.Equals(ActivatableEntity.NormalizeSpaces(p.PositionName), positionName, StringComparison.OrdinalIgnoreCase)))
            throw new CompanyException("Já existe um cargo com esse nome.");

        var position = CompanyPosition.Create(
            companyId: Id,
            positionName: positionName,
            accessLevel: accessLevel,
            isSystem: isSystem
        );

        _companyPositions.Add(position);
        return position;
    }

    public void AssignServiceToPosition(Guid positionId, Guid serviceOfferingId)
    {
        var position = _companyPositions.SingleOrDefault(p => p.Id == positionId && p.IsActive)
            ?? throw new CompanyException("Cargo não encontrado ou inativo.");

        var service = _companyServiceOfferings.SingleOrDefault(s => s.Id == serviceOfferingId)
            ?? throw new CompanyException("Serviço não encontrado.");

        if (position.CompanyId != Id || service.CompanyId != Id)
            throw new CompanyException("Serviço e cargo devem pertencer à mesma empresa.");

        if (!position.ServiceOfferings.Contains(service))
            position.ServiceOfferings.Add(service);
    }

    public bool RenamePosition(Guid positionId, string newName)
    {
        newName = ActivatableEntity.NormalizeSpaces(newName);
        var pos = _companyPositions.SingleOrDefault(p => p.Id == positionId)
            ?? throw new CompanyException("Cargo não encontrado.");

        if (_companyPositions.Any(p => p.Id != positionId &&
                string.Equals(ActivatableEntity.NormalizeSpaces(p.PositionName), newName, StringComparison.OrdinalIgnoreCase)))
            throw new CompanyException("Já existe um cargo com esse nome.");

        return pos.Rename(newName);
    }

    public bool DeactivatePosition(Guid positionId)
    {
        var pos = _companyPositions.SingleOrDefault(p => p.Id == positionId)
            ?? throw new CompanyException("Cargo não encontrado.");

        if (_members.Any(m => m.PrimaryPositionId == pos.Id && m.IsActive))
            throw new CompanyException("Não é possível desativar um cargo em uso.");

        return pos.Deactivate();
    }

    public bool ActivatePosition(Guid positionId)
    {
        var pos = _companyPositions.SingleOrDefault(p => p.Id == positionId)
            ?? throw new CompanyException("Cargo não encontrado.");

        return pos.Activate();
    }

    // ===== Service Offerings (ONE-TO-MANY) =====
    public CompanyServiceOffering AddServiceOffering(string serviceName, Money price)
    {
        var service = CompanyServiceOffering.Create(Id, serviceName, price);

        _companyServiceOfferings.Add(service);

        return service;
    }

    public bool RenameServiceOffering(Guid serviceOfferingId, string newName)
    {
        newName = ActivatableEntity.NormalizeSpaces(newName);
        var svc = _companyServiceOfferings.SingleOrDefault(s => s.Id == serviceOfferingId)
            ?? throw new CompanyException("Serviço não encontrado.");

        // Se a sua unicidade for global por empresa (CompanyId + ServiceName)
        if (_companyServiceOfferings.Any(s => s.Id != serviceOfferingId &&
               string.Equals(ActivatableEntity.NormalizeSpaces(s.ServiceOfferingName), newName, StringComparison.OrdinalIgnoreCase)))
            throw new CompanyException("Já existe um serviço com esse nome.");

        return svc.Rename(newName);
    }

    public void RemoveServiceOffering(Guid serviceOfferingId)
    {
        var svc = _companyServiceOfferings.SingleOrDefault(s => s.Id == serviceOfferingId)
            ?? throw new CompanyException("Serviço não encontrado.");
        _companyServiceOfferings.Remove(svc);
    }

    // ===== Helpers =====

    private CompanyPosition EnsurePositionByName(string positionName, CompanyRole accessLevel, bool isSystem = false)
    {
        var position = _companyPositions
            .FirstOrDefault(p => p.PositionName.Equals(positionName, StringComparison.OrdinalIgnoreCase));


        if (position != null)
            return position;

        position = CompanyPosition.Create(
            companyId: Id,
            positionName: positionName,
            accessLevel: accessLevel,
            isSystem: isSystem
        );

        _companyPositions.Add(position);

        return position;
    }
}