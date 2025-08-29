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

    private CompanyMember() { }

    private CompanyMember(Guid companyId, Guid userId)
    {
        if (companyId == Guid.Empty)
            throw new CompanyException("CompanyId inválido.");

        if (userId == Guid.Empty)
            throw new CompanyException("UserId inválido.");

        CompanyId = companyId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
    }

    internal static CompanyMember CreateFounder(Guid companyId, Guid userId)
    {
        var member = new CompanyMember(companyId, userId)
        {
            Role = CompanyRole.Owner
        };

        return member;
    }

    internal static CompanyMember CreateMember(Guid companyId, Guid userId, CompanyRole role)
    {
        if (role == CompanyRole.Owner)
            throw new CompanyException("Metodo inválido para criação de Owner.");

        var member = new CompanyMember(companyId, userId);
        member.Role = role;
        return member;
    }


    public (bool changed, bool requiresSessionRefresh) ChangeRole(CompanyRole newRole)
        => SetRole(newRole, allowOwnerLevel: false);

    internal (bool changed, bool requiresSessionRefresh) SetRoleFromPosition(CompanyPosition position)
    {
        var allowOwnerLevel = position.IsSystem && position.AccessLevel == CompanyRole.Owner;
        return SetRole(position.AccessLevel, allowOwnerLevel);
    }

    private (bool changed, bool requiresSessionRefresh) SetRole(CompanyRole newRole, bool allowOwnerLevel)
    {
        if (newRole == CompanyRole.Owner && !allowOwnerLevel)
            throw new CompanyException("Promoção a Owner é proibida. Owner só no ato de criar a empresa.");

        if (Role == CompanyRole.Owner && !allowOwnerLevel)
            throw new CompanyException("O Founder/Owner não pode ter o role alterado.");

        if (newRole == Role)
            return (false, false);

        Role = newRole;
        return (true, true);
    }

    public void SetPrimaryPosition(CompanyPosition position)
    {
        if (position.CompanyId != CompanyId)
            throw new CompanyException("Cargo não pertence a esta empresa.");

        if (!position.IsActive)
            throw new CompanyException("Não é possível atribuir cargo inativo.");

        PrimaryPositionId = position.Id;
        PrimaryPosition = position;

        SetRoleFromPosition(position);
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
        if (Role == CompanyRole.Owner)
            return CompanyPermission.All;

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

}