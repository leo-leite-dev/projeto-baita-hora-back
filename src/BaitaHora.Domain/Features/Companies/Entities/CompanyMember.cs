using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class CompanyMember : Entity
{
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public CompanyRole Role { get; private set; }

    public Guid? PrimaryPositionId { get; private set; }
    public CompanyPosition? PrimaryPosition { get; private set; }

    public CompanyPermission DirectPermissionMask { get; private set; } = CompanyPermission.None;

    public DateTime JoinedAt { get; private set; }
    public bool IsActive { get; private set; }

    private CompanyMember() { }

    private CompanyMember(Guid companyId, Guid userId)
    {
        if (companyId == Guid.Empty) throw new CompanyException("CompanyId inválido.");
        if (userId == Guid.Empty) throw new CompanyException("UserId inválido.");

        CompanyId = companyId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static CompanyMember CreateMember(Guid companyId, Guid userId, CompanyRole role)
    {
        if (role == CompanyRole.Owner)
            throw new CompanyException("Use CreateFounder para criar Owner.");

        var member = new CompanyMember(companyId, userId);
        member.SetRole(role, allowOwnerLevel: false);
        return member;
    }

    public static CompanyMember CreateFounder(Guid companyId, Guid userId)
    {
        var member = new CompanyMember(companyId, userId);
        member.SetRole(CompanyRole.Owner, allowOwnerLevel: true);
        return member;
    }

    public bool SetRole(CompanyRole newRole, bool allowOwnerLevel = false)
    {
        if (newRole == Role) return false;
        if (newRole == CompanyRole.Owner && !allowOwnerLevel)
            throw new CompanyException("Elevação para Owner não autorizada.");

        Role = newRole;
        Touch();
        return true;
    }

    public bool SetPrimaryPosition(CompanyPosition position)
    {
        if (position.CompanyId != CompanyId)
            throw new CompanyException("O cargo pertence a outra empresa.");
        if (!position.IsActive)
            throw new CompanyException("Não é possível atribuir um cargo inativo.");

        PrimaryPositionId = position.Id;
        PrimaryPosition = position;
        Touch();
        return true;
    }

    public bool ClearPrimaryPosition()
    {
        if (PrimaryPositionId is null) return false;

        PrimaryPositionId = null;
        PrimaryPosition = null;
        Touch();
        return true;
    }

    public CompanyPermission GetEffectivePermissions()
    {
        if (Role == CompanyRole.Owner) return CompanyPermission.All;

        var mask = CompanyPermission.None;
        if (PrimaryPosition is not null)
            mask |= PrimaryPosition.PermissionMask;
        mask |= DirectPermissionMask;
        return mask;
    }

    public bool SetDirectPermissions(CompanyPermission newMask)
    {
        if (DirectPermissionMask == newMask) return false;
        DirectPermissionMask = newMask;
        Touch();
        return true;
    }

    public bool AddDirectPermissions(CompanyPermission extra)
        => SetDirectPermissions(DirectPermissionMask | extra);

    public bool RemoveDirectPermissions(CompanyPermission remove)
        => SetDirectPermissions(DirectPermissionMask & ~remove);

    public bool ClearDirectPermissions()
        => SetDirectPermissions(CompanyPermission.None);

    public bool SetActive(bool isActive)
    {
        if (IsActive == isActive) return false;
        IsActive = isActive;
        Touch();
        return true;
    }

    public bool Activate() => SetActive(true);
    public bool Deactivate() => SetActive(false);
}