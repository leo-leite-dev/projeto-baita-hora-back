using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class CompanyPosition : Entity
{
    public Guid CompanyId { get; private set; }
    public string PositionName { get; private set; } = null!;
    public CompanyPermission PermissionMask { get; private set; }
    public CompanyRole AccessLevel { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSystem { get; private set; }

    public ICollection<CompanyServiceOffering> ServiceOfferings { get; private set; } = new List<CompanyServiceOffering>();
    private readonly List<CompanyMember> _members = new();
    public IReadOnlyCollection<CompanyMember> Members => _members;

    private CompanyPosition() { }

    internal static CompanyPosition Create(Guid companyId, string positionName, CompanyRole accessLevel, bool isSystem = false, CompanyPermission permissionMask = CompanyPermission.None)
    {
        if (companyId == Guid.Empty)
            throw new CompanyException("CompanyId inválido.");

        if (string.IsNullOrWhiteSpace(positionName))
            throw new CompanyException("Nome do cargo é obrigatório.");

        if (!Enum.IsDefined(typeof(CompanyRole), accessLevel) || accessLevel == CompanyRole.Unknown)
            throw new CompanyException("Nível de acesso inválido.");

        if (accessLevel == CompanyRole.Owner && !isSystem)
            throw new CompanyException("Cargo 'Owner' não existe; Owner apenas para o fundador (sistema).");

        var position = new CompanyPosition
        {
            PositionName = positionName,
            CompanyId = companyId,
            AccessLevel = accessLevel,
            IsActive = true,
            IsSystem = isSystem,
            PermissionMask = permissionMask
        };


        return position;
    }

    public bool Rename(string newName)
    {
        var normalized = newName?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new CompanyException("Nome do cargo é obrigatório.");

        var isFounderName = string.Equals(normalized, "Fundador", StringComparison.OrdinalIgnoreCase);
        var isFounderSystem = IsSystem && AccessLevel == CompanyRole.Owner;

        if (isFounderName && !isFounderSystem)
            throw new CompanyException("Cargo 'Fundador' só é permitido para a posição de sistema do fundador.");

        if (string.Equals(PositionName, normalized, StringComparison.Ordinal))
            return false;

        PositionName = normalized;
        return true;
    }

    public void ChangeAccessLevel(CompanyRole newLevel)
    {
        if (!Enum.IsDefined(typeof(CompanyRole), newLevel) || newLevel == CompanyRole.Unknown)
            throw new CompanyException("Nível de acesso inválido.");

        if (newLevel == CompanyRole.Owner && !IsSystem)
            throw new CompanyException("Nível Owner só é permitido para a posição de sistema do fundador.");

        AccessLevel = newLevel;
    }

    public void SetPermissionMask(CompanyPermission newMask)
    {
        if (IsSystem && AccessLevel == CompanyRole.Owner)
        {
            PermissionMask = CompanyPermission.All;
            return;
        }

        PermissionMask = newMask;
    }

    public bool Deactivate()
    {
        if (!IsActive) return false;

        IsActive = false;
        return true;
    }

    public bool Activate()
    {
        if (IsActive) return false;
        IsActive = true;
        return true;
    }
}