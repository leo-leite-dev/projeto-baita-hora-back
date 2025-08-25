using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;

public sealed class CompanyPosition : Entity
{
    public Guid CompanyId { get; private set; }
    public string PositionName { get; private set; } = null!;
    public CompanyPermission PermissionMask { get; private set; }
    public CompanyRole AccessLevel { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystem { get; private set; }

    private CompanyPosition() { }

    internal static CompanyPosition Create(Guid companyId, string positionName, CompanyRole accessLevel, bool isSystem = false)
    {
        if (companyId == Guid.Empty)
            throw new CompanyException("CompanyId inválido.");

        if (string.IsNullOrWhiteSpace(positionName))
            throw new CompanyException("Nome do cargo é obrigatório.");

        if (!Enum.IsDefined(typeof(CompanyRole), accessLevel) || accessLevel == CompanyRole.Unknown)
            throw new CompanyException("Nível de acesso inválido.");

        if (accessLevel == CompanyRole.Owner && !isSystem)
            throw new CompanyException("Cargo 'Owner' não existe; Owner só no fundador.");

        var position = new CompanyPosition
        {
            CompanyId = companyId,
            AccessLevel = accessLevel,
            IsActive = true,
            IsSystem = isSystem
        };

        position.SetName(positionName);
        return position;
    }

    public bool SetName(string newName)
    {
        var normalized = newName?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new CompanyException("Nome do cargo é obrigatório.");

        var isFourderName = string.Equals(normalized, "Fundador", StringComparison.OrdinalIgnoreCase);
        var isFouderSystem = IsSystem && AccessLevel == CompanyRole.Owner;

        if (isFourderName && !isFouderSystem)
            throw new CompanyException("Cargo 'Fundador' não pode ser criado ou alterado.");

        if (string.Equals(PositionName, normalized, StringComparison.Ordinal))
            return false;

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
