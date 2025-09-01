using BaitaHora.Domain.Common.ValueObjects;
using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class Company : Entity
{
    public CNPJ Cnpj { get; private set; } = default!;
    public Address Address { get; private set; } = default!;
    public Phone CompanyPhone { get; private set; }
    public Email CompanyEmail { get; private set; }

    public string? TradeName { get; private set; } = string.Empty;

    public CompanyImage? Image { get; private set; }

    private readonly List<CompanyMember> _members = new();
    public IReadOnlyCollection<CompanyMember> Members => _members.AsReadOnly();

    private readonly List<CompanyPosition> _positions = new();
    public IReadOnlyCollection<CompanyPosition> Positions => _positions.AsReadOnly();

    private readonly List<CompanyServiceOffering> _serviceOfferings = new();
    public IReadOnlyCollection<CompanyServiceOffering> ServiceOfferings => _serviceOfferings.AsReadOnly();

    private Company() { }

    public static Company Create(string companyName, CNPJ cnpj, Address address, string? tradeName, Phone companyPhone, Email companyEmail)
    {
        if (address is null)
            throw new CompanyException("Endereço é obrigatório.");

        var company = new Company
        {
            Name = companyName.Trim(),
            Cnpj = cnpj,
            Address = address,
            TradeName = NormalizeSpaces(tradeName),
            CompanyPhone = companyPhone,
            CompanyEmail = companyEmail,
        };

        company.ValidateInvariants();
        return company;
    }

    public bool Rename(string newName)
    {
        if (Name.Equals(newName))
            return false;

        ValidateInvariants();
        Name = newName;
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
        var normalized = NormalizeSpaces(newTradeName);

        if (string.Equals(NormalizeSpaces(TradeName), normalized, StringComparison.Ordinal))
            return false;

        ValidateInvariants();
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

        if (string.IsNullOrWhiteSpace(Name))
            throw new CompanyException("Nome completo é obrigatório.");

        if (Name.Length < 3 || Name.Length > 120)
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

    // ===== Member =====

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
        var newPosition = _positions.SingleOrDefault(p => p.Id == newPositionId && p.IsActive)
            ?? throw new CompanyException("Cargo não encontrado ou inativo.");

        var member = _members.SingleOrDefault(m => m.UserId == userId && m.IsActive)
            ?? throw new CompanyException("Membro não encontrado ou inativo.");

        member.SetPrimaryPosition(newPosition);

        if (alignRoleToPosition)
            member.ChangeRole(newPosition.AccessLevel);

        return member;
    }

    // ===== Positions =====

    public CompanyPosition AddPosition(string positionName, CompanyRole accessLevel, IEnumerable<Guid> serviceOfferingIds, bool isSystem = false, CompanyPermission permissionMask = CompanyPermission.None)
    {
        var normalized = NormalizeSpaces(positionName);

        if (_positions.Any(p => p.IsActive &&
            string.Equals(NormalizeSpaces(p.Name), normalized, StringComparison.OrdinalIgnoreCase)))
            throw new CompanyException("Já existe um cargo com esse nome.");

        var ids = (serviceOfferingIds ?? Enumerable.Empty<Guid>())
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
            throw new CompanyException("Um cargo deve ter ao menos um serviço associado.");

        var services = _serviceOfferings.Where(s => ids.Contains(s.Id)).ToList();

        if (services.Count != ids.Length)
            throw new CompanyException("Um ou mais serviços não foram encontrados.");

        if (services.Any(s => s.CompanyId != Id || !s.IsActive))
            throw new CompanyException("Serviços devem pertencer à empresa e estar ativos.");

        var position = CompanyPosition.Create(Id, normalized, accessLevel, isSystem, permissionMask);

        position.AddServiceOfferings(services);

        _positions.Add(position);
        return position;
    }

    public void AssignServicesToPosition(Guid positionId, IEnumerable<Guid> serviceOfferingIds)
    {
        var position = _positions.SingleOrDefault(p => p.Id == positionId && p.IsActive)
            ?? throw new CompanyException("Cargo não encontrado ou inativo.");

        var ids = (serviceOfferingIds ?? Enumerable.Empty<Guid>())
                  .Where(x => x != Guid.Empty)
                  .Distinct()
                  .ToArray();

        if (ids.Length == 0)
            throw new CompanyException("Um cargo deve ter ao menos um serviço associado.");

        var services = _serviceOfferings.Where(s => ids.Contains(s.Id)).ToList();

        if (services.Count != ids.Length)
            throw new CompanyException("Um ou mais serviços não foram encontrados.");

        if (services.Any(s => s.CompanyId != Id || !s.IsActive))
            throw new CompanyException("Serviços devem pertencer à empresa e estar ativos.");

        position.ReplaceServiceOfferings(services);
    }

    public bool RenamePosition(Guid positionId, string newName)
    {
        newName = NormalizeSpaces(newName);
        var position = _positions.SingleOrDefault(p => p.Id == positionId)
            ?? throw new CompanyException("Cargo não encontrado.");

        if (_positions.Any(p => p.Id != positionId &&
                string.Equals(NormalizeSpaces(p.Name), newName, StringComparison.OrdinalIgnoreCase)))
            throw new CompanyException("Já existe um cargo com esse nome.");

        return position.Rename(newName);
    }

    public CompanyPosition ChangePositionAccessLevel(Guid positionId, CompanyRole newLevel, bool alignMembers = false)
    {
        var position = _positions.SingleOrDefault(p => p.Id == positionId && p.IsActive)
            ?? throw new CompanyException("Cargo não encontrado ou inativo.");

        if (position.IsSystem && position.AccessLevel == CompanyRole.Owner && newLevel != CompanyRole.Owner)
            throw new CompanyException("Nível do cargo 'Fundador' do sistema não pode ser alterado.");

        position.ChangeAccessLevel(newLevel);

        if (alignMembers)
        {
            foreach (var m in _members.Where(m => m.IsActive && m.PrimaryPositionId == positionId))
                m.ChangeRole(newLevel);
        }

        return position;
    }

    public void RemovePosition(Guid positionId)
    {
        var position = _positions.SingleOrDefault(p => p.Id == positionId)
            ?? throw new CompanyException("Cargo não encontrado.");

        if (position.IsSystem)
            throw new CompanyException("Cargo do sistema não pode ser removido.");

        if (_members.Any(m => m.PrimaryPositionId == positionId && m.IsActive))
            throw new CompanyException("Não é possível remover um cargo que ainda possui membros ativos.");

        _positions.Remove(position);
    }

    public void RemoveServicesFromPosition(Guid positionId, IReadOnlyCollection<Guid> serviceOfferingIds)
    {
        var position = _positions.SingleOrDefault(p => p.Id == positionId && p.IsActive)
            ?? throw new CompanyException("Cargo não encontrado ou inativo.");

        position.RemoveServiceOfferings(serviceOfferingIds);
    }

    // ===== Service Offerings (ONE-TO-MANY) =====
    public CompanyServiceOffering AddServiceOffering(string serviceName, Money price)
    {
        if (!IsActive)
            throw new CompanyException("Não é possível adicionar serviços em uma empresa inativa.");

        var normalized = NormalizeAndValidateName(serviceName);

        var exists = _serviceOfferings.Any(s => s.IsActive && NameEquals(s.Name, normalized));

        var service = CompanyServiceOffering.Create(Id, serviceName, price);

        if (exists)
            throw new CompanyException($"Já existe um serviço ativo com o nome '{normalized}'.");

        _serviceOfferings.Add(service);

        return service;
    }

    public bool RenameServiceOffering(Guid serviceOfferingId, string newName)
    {
        var service = _serviceOfferings.SingleOrDefault(s => s.Id == serviceOfferingId)
            ?? throw new CompanyException("Serviço não encontrado.");

        var normalized = NormalizeAndValidateName(newName);

        var exists = _serviceOfferings.Any(s =>
         s.IsActive && s.Id != serviceOfferingId && NameEquals(s.Name, normalized));

        if (exists)
            throw new CompanyException($"Já existe um serviço ativo com o nome '{normalized}'.");

        return service.Rename(normalized);
    }

    public void RemoveServiceOffering(Guid serviceOfferingId)
    {
        var service = _serviceOfferings.SingleOrDefault(s => s.Id == serviceOfferingId)
            ?? throw new CompanyException("Serviço não encontrado.");

        _serviceOfferings.Remove(service);
    }

    // dentro de Company
    public void DetachServiceOfferingsFromAllPositions(IReadOnlyCollection<Guid> serviceOfferingIds)
    {
        var ids = (serviceOfferingIds ?? Array.Empty<Guid>())
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        if (ids.Length == 0) return;

        var blockers = _positions
            .Where(p => p.IsActive)
            .Where(p => p.ServiceOfferings.Any())                       
            .Where(p => p.ServiceOfferings.All(s => ids.Contains(s.Id)))  
            .Select(p => $"{p.Name} ({p.Id})")
            .ToArray();

        if (blockers.Length > 0)
            throw new CompanyException(
                "Não é possível desativar esses serviços porque deixariam cargos sem serviços: " +
                string.Join(", ", blockers));

        foreach (var pos in _positions.Where(p => p.IsActive))
        {
            var toRemove = pos.ServiceOfferings
                .Where(s => ids.Contains(s.Id))
                .Select(s => s.Id)
                .ToArray();

            if (toRemove.Length > 0)
                RemoveServicesFromPosition(pos.Id, toRemove);
        }
    }

    // ===== Helpers =====

    private CompanyPosition EnsurePositionByName(string positionName, CompanyRole accessLevel, bool isSystem = false)
    {
        var position = _positions
            .FirstOrDefault(p => p.Name.Equals(positionName, StringComparison.OrdinalIgnoreCase));


        if (position != null)
            return position;

        position = CompanyPosition.Create(
            companyId: Id,
            positionName: positionName,
            accessLevel: accessLevel,
            isSystem: isSystem
        );

        _positions.Add(position);

        return position;
    }
}