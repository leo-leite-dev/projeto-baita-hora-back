using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Domain.Features.Commons;
using BaitaHora.Domain.Features.Commons.Exceptions;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class Company : Entity
{
    public CompanyName CompanyName { get; private set; }
    public CNPJ Cnpj { get; private set; }
    public Address Address { get; private set; } = default!;
    public Phone CompanyPhone { get; private set; }
    public Email CompanyEmail { get; private set; }

    public string? TradeName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public CompanyImage? Image { get; private set; }

    private readonly List<CompanyMember> _members = new();
    public IReadOnlyCollection<CompanyMember> Members => _members.AsReadOnly();

    private readonly List<CompanyPosition> _companyPositions = new();
    public IReadOnlyCollection<CompanyPosition> Positions => _companyPositions.AsReadOnly();

    private Company() { }

    public static Company Create(CompanyName companyName, CNPJ cnpj, Address address, string? tradeName, Phone CompanyPhone, Email CompanyEmail)
    {
        if (address is null) throw new CompanyException("Endereço é obrigatório.");

        var company = new Company();
        company.SetName(companyName);
        company.SetCnpj(cnpj);
        company.SetAddress(address);
        company.SetTradeName(tradeName);
        company.SetPhone(CompanyPhone);
        company.SetEmail(CompanyEmail);

        // company.SetImage(image);

        return company;
    }

    public bool SetName(CompanyName newCompanyName)
    {
        if (CompanyName.Equals(newCompanyName)) return false;
        CompanyName = newCompanyName;
        return true;
    }

    public bool SetCnpj(CNPJ newCnpj)
    {
        if (Cnpj.Equals(newCnpj)) return false;
        Cnpj = newCnpj;
        return true;
    }

    public bool SetAddress(Address newAddress)
    {
        if (newAddress is null) throw new CompanyException("Endereço é obrigatório.");
        if (Address != null && Address.Equals(newAddress)) return false;

        Address = newAddress;
        return true;
    }

    public bool SetPhone(Phone newPhone)
    {
        if (CompanyPhone.Equals(newPhone)) return false;
        CompanyPhone = newPhone;
        return true;
    }

    public bool SetEmail(Email newEmail)
    {
        if (CompanyEmail.Equals(newEmail)) return false;
        CompanyEmail = newEmail;
        return true;
    }

    public bool SetTradeName(string? newTradeName)
    {
        var normalized = string.IsNullOrWhiteSpace(newTradeName) ? null : newTradeName.Trim();
        if (string.Equals(TradeName, normalized, StringComparison.Ordinal)) return false;

        if (normalized is not null && normalized.Length > 200)
            throw new CompanyException("Nome fantasia deve ter no máximo 200 caracteres.");

        TradeName = normalized;
        return true;
    }

    public bool SetImage(CompanyImage? newImage)
    {
        if (newImage is null)
        {
            if (Image is null) return false;
            Image = null;
            return true;
        }

        if (Image is not null && Equals(Image, newImage)) return false;
        Image = newImage;
        return true;
    }

    public CompanyMember AddOwnerFounder(Guid userId)
    {
        if (_members.Any(m => m.Role == CompanyRole.Owner && m.IsActive))
            throw new CompanyException("Já existe um Owner para esta empresa.");

        var member = CompanyMember.Create(Id, userId, CompanyRole.Owner);

        var founder = EnsurePositionByName(
            name: "Fundador",
            accessLevel: CompanyRole.Owner,
            allowOwnerLevel: true,
            isSystem: true
        );
        member.SetPrimaryPosition(founder);

        _members.Add(member);

        return member;
    }

    public CompanyMember AddMemberFromPosition(Guid userId, CompanyPosition position)
    {
        if (position is null)
            throw new CompanyException("Posição é obrigatória.");
        if (position.CompanyId != Id)
            throw new CompanyException("Cargo não pertence a esta empresa.");
        if (!position.IsActive)
            throw new CompanyException("Não é possível atribuir um cargo inativo.");
        if (position.AccessLevel == CompanyRole.Owner)
            throw new CompanyException("Cargo de nível Owner só pode ser atribuído ao fundador.");

        var member = CompanyMember.Create(Id, userId, position.AccessLevel);
        member.SetPrimaryPosition(position);

        _members.Add(member);
        return member;
    }

    public CompanyMember ChangeMemberPosition(Guid userId, CompanyPosition newPosition, bool alignRoleToPosition = true)
    {
        if (newPosition is null)
            throw new CompanyException("Posição é obrigatória.");
        if (newPosition.CompanyId != Id)
            throw new CompanyException("Cargo não pertence a esta empresa.");
        if (!newPosition.IsActive) throw
         new CompanyException("Não é possível atribuir um cargo inativo.");
        if (newPosition.AccessLevel == CompanyRole.Owner)
            throw new CompanyException("Cargo de nível Owner só pode ser atribuído ao fundador.");

        var member = _members.SingleOrDefault(m => m.UserId == userId && m.IsActive)
            ?? throw new CompanyException("Membro não encontrado ou inativo.");

        member.SetPrimaryPosition(newPosition);

        if (alignRoleToPosition)
            member.SetRole(newPosition.AccessLevel);

        return member;
    }

    public CompanyPosition AddPosition(string name, CompanyRole accessLevel, bool isSystem = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CompanyException("Nome do cargo é obrigatório.");

        if (_companyPositions.Any(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
            throw new CompanyException("Já existe um cargo com esse nome.");

        if (accessLevel == CompanyRole.Owner)
            throw new CompanyException("Cargo de nível Owner só pode ser criado pelo fluxo de fundador.");

        var pos = CompanyPosition.Create(
            companyId: Id,
            name: name,
            accessLevel: accessLevel,
            allowOwnerLevel: false,
            isSystem: isSystem
        );

        _companyPositions.Add(pos);
        return pos;
    }

    public bool RenamePosition(Guid positionId, string newName)
    {
        var pos = _companyPositions.SingleOrDefault(p => p.Id == positionId)
            ?? throw new CompanyException("Cargo não encontrado.");

        if (_companyPositions.Any(p => p.Id != positionId &&
                string.Equals(p.Name, newName, StringComparison.OrdinalIgnoreCase)))
            throw new CompanyException("Já existe um cargo com esse nome.");

        return pos.SetName(newName);
    }

    public bool ChangePositionAccessLevel(Guid positionId, CompanyRole newLevel)
    {
        var pos = _companyPositions.SingleOrDefault(p => p.Id == positionId)
            ?? throw new CompanyException("Cargo não encontrado.");

        if (newLevel == CompanyRole.Owner)
            throw new CompanyException("Nível Owner é reservado ao fundador.");

        return pos.SetAccessLevel(newLevel);
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

    private CompanyPosition EnsurePositionByName(string name, CompanyRole accessLevel, bool allowOwnerLevel = false, bool isSystem = false)
    {
        var pos = _companyPositions.FirstOrDefault(
            p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

        if (pos is not null) return pos;

        pos = CompanyPosition.Create(
            companyId: Id,
            name: name,
            accessLevel: accessLevel,
            allowOwnerLevel: allowOwnerLevel,
            isSystem: isSystem
        );

        _companyPositions.Add(pos);
        return pos;
    }
}