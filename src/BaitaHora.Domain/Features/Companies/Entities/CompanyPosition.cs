using BaitaHora.Domain.Features.Commons;
using BaitaHora.Domain.Features.Commons.Exceptions;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;

public class CompanyPosition : Entity
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = null!;
    public CompanyRole AccessLevel { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystem { get; private set; }
    public Company Company { get; private set; } = null!;

    private CompanyPosition() { }

    public static CompanyPosition Create(
        Guid companyId, string name, CompanyRole accessLevel,
        bool allowOwnerLevel = false, bool isSystem = false)
    {
        if (companyId == Guid.Empty) throw new CompanyException("CompanyId inválido.");
        if (accessLevel == CompanyRole.Owner && !allowOwnerLevel)
            throw new CompanyException("Cargo Owner só pode ser criado no fluxo do fundador.");

        var pos = new CompanyPosition
        {
            CompanyId = companyId,
            AccessLevel = accessLevel,
            IsActive = true,
            IsSystem = isSystem
        };

        pos.SetName(name);               
        return pos;
    }

    public bool SetName(string newName)
    {
        var normalized = newName?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new CompanyException("Nome do cargo é obrigatório.");
        if (string.Equals(Name, normalized, StringComparison.Ordinal)) return false;
        Name = normalized; Touch(); return true;
    }

    public bool SetAccessLevel(CompanyRole newLevel, bool allowOwnerLevel = false)
    {
        if (newLevel == AccessLevel) return false;
        if (newLevel == CompanyRole.Owner && !allowOwnerLevel)
            throw new CompanyException("Nível Owner é reservado ao fundador.");
        AccessLevel = newLevel; Touch(); return true;
    }

    public bool Deactivate() { if (!IsActive) return false; IsActive = false; Touch(); return true; }
    public bool Activate() { if (IsActive) return false; IsActive = true; Touch(); return true; }
}
