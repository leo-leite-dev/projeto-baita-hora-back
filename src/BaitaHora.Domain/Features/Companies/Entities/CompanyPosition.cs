using BaitaHora.Domain.Features.Commons;
using BaitaHora.Domain.Features.Commons.Exceptions;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;

public class CompanyPosition : Entity
{
    public Guid CompanyId { get; private set; }
    public string PositionName { get; private set; } = null!;
    public CompanyPermission PermissionMask { get; private set; }
    public CompanyRole AccessLevel { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystem { get; private set; }
    public Company Company { get; private set; } = null!;

    private CompanyPosition() { }

    public static CompanyPosition Create(Guid companyId, string positionName, CompanyRole accessLevel,
    bool allowOwnerLevel = false, bool isSystem = false)
    {
        if (companyId == Guid.Empty) throw new CompanyException("CompanyId inválido.");
        if (accessLevel == CompanyRole.Owner && !allowOwnerLevel)
            throw new CompanyException("Cargo Owner só pode ser criado no fluxo do fundador.");

        var companyPosition = new CompanyPosition
        {
            CompanyId = companyId,
            AccessLevel = accessLevel,
            IsActive = true,
            IsSystem = isSystem
        };

        companyPosition.SetName(positionName);
        return companyPosition;
    }

    public bool SetName(string newName)
    {
        var normalized = newName?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new CompanyException("Nome do cargo é obrigatório.");

        if (string.Equals(PositionName, normalized, StringComparison.Ordinal)) return false;

        PositionName = normalized;
        return true;
    }

    public bool Deactivate()
    {
        if (!IsActive) return false;
        return true;
    }
    public bool Activate()
    {
        if (IsActive) return false;
        IsActive = true;
        return true;
    }
}
