using BaitaHora.Domain.Commons;
using BaitaHora.Domain.Commons.Exceptions;
using BaitaHora.Domain.Commons.ValueObjects;
using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Domain.Entities.Companies;
using BaitaHora.Domain.Enums;

public sealed class Company : Entity
{
    public CompanyName CompanyName { get; private set; }
    public CNPJ Cnpj { get; private set; }
    public Address Address { get; private set; } = default!;
    public Phone Phone { get; private set; }
    public Email Email { get; private set; }

    public string? TradeName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public CompanyImage? Image { get; private set; }

    private readonly List<CompanyMember> _members = new();
    public IReadOnlyCollection<CompanyMember> Members => _members.AsReadOnly();

    private readonly List<CompanyPosition> _positions = new();
    public IReadOnlyCollection<CompanyPosition> Positions => _positions.AsReadOnly();

    private Company() { }

    public static Company Create(CompanyName companyName, CNPJ cnpj, Address address, string? tradeName, Phone phone, Email email)
    {
        if (address is null) throw new CompanyException("Endereço é obrigatório.");

        var company = new Company
        {
            CompanyName = companyName,
            Cnpj = cnpj,
            Address = address,
            Phone = phone,
            Email = email,
            TradeName = string.IsNullOrWhiteSpace(tradeName) ? null : tradeName.Trim()
        };

        company.Touch();
        return company;
    }

    public bool SetName(CompanyName newCompanyName)
    {
        if (CompanyName != null && CompanyName.Equals(newCompanyName)) return false;
        CompanyName = newCompanyName;
        Touch();
        return true;
    }

    public bool SetCnpj(CNPJ newCnpj)
    {
        if (Cnpj != null && Cnpj.Equals(newCnpj)) return false;
        Cnpj = newCnpj;
        Touch();
        return true;
    }

    public bool SetAddress(Address newAddress)
    {
        if (newAddress is null) throw new CompanyException("Endereço é obrigatório.");
        if (Address != null && Address.Equals(newAddress)) return false;

        Address = newAddress;
        Touch();
        return true;
    }

    public bool SetPhone(Phone newPhone)
    {
        if (Phone != null && Phone.Equals(newPhone)) return false;
        Phone = newPhone;
        Touch();
        return true;
    }

    public bool SetEmail(Email newEmail)
    {
        if (Email != null && Email.Equals(newEmail)) return false;
        Email = newEmail;
        Touch();
        return true;
    }

    public bool SetTradeName(string? newTradeName)
    {
        var normalized = string.IsNullOrWhiteSpace(newTradeName) ? null : newTradeName.Trim();
        if (string.Equals(TradeName, normalized, StringComparison.Ordinal)) return false;

        if (normalized is not null && normalized.Length > 200)
            throw new CompanyException("Nome fantasia deve ter no máximo 200 caracteres.");

        TradeName = normalized;
        Touch();
        return true;
    }

    public bool SetImage(CompanyImage? newImage)
    {
        if (newImage is null)
        {
            if (Image is null) return false;
            Image = null;
            Touch();
            return true;
        }

        if (Image is not null && Equals(Image, newImage)) return false;
        Image = newImage;
        Touch();
        return true;
    }

    public bool SetActive(bool newActive)
    {
        if (IsActive == newActive) return false;
        IsActive = newActive;
        Touch();
        return true;
    }

    public CompanyPosition CreatePosition(string name, CompanyRole level, bool allowOwnerLevel = false)
    {
        var normalized = NormalizeNameRequired(name);

        if (level == CompanyRole.Unknown)
            throw new CompanyException("Nível de cargo inválido.");

        if (level == CompanyRole.Owner && !allowOwnerLevel)
            throw new CompanyException("Cargo de Owner não permitido.");

        if (_positions.Any(p => string.Equals(p.Name, normalized, StringComparison.OrdinalIgnoreCase)))
            throw new CompanyException($"Já existe cargo '{normalized}' nesta empresa.");

        var pos = new CompanyPosition(Id, normalized, level, allowOwnerLevel);
        _positions.Add(pos);
        Touch();
        return pos;
    }

    public bool RenamePosition(Guid positionId, string newName)
    {
        var pos = _positions.FirstOrDefault(p => p.Id == positionId)
            ?? throw new CompanyException("Cargo não encontrado.");

        var normalized = NormalizeNameRequired(newName);

        if (_positions.Any(p => p.Id != positionId &&
                                string.Equals(p.Name, normalized, StringComparison.OrdinalIgnoreCase)))
            throw new CompanyException($"Já existe cargo '{normalized}' nesta empresa.");

        if (string.Equals(pos.Name, normalized, StringComparison.OrdinalIgnoreCase))
            return false;

        pos.Rename(normalized);
        Touch();
        return true;
    }

    private static string NormalizeNameRequired(string? name)
    {
        var normalized = (name ?? string.Empty).Trim();
        if (normalized.Length == 0) throw new CompanyException("Nome obrigatório.");
        return normalized;
    }
}