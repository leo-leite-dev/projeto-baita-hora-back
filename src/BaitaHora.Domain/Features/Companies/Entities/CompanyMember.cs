using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Domain.Features.Companies.Entities;

public sealed class CompanyMember : EntityBase
{
    public Guid CompanyId { get; private set; }
    public Guid UserId { get; private set; }

    public User User { get; private set; } = null!;
    public CompanyRole Role { get; private set; }

    public Guid PrimaryPositionId { get; private set; }
    public CompanyPosition PrimaryPosition { get; private set; } = null!;

    public CompanyPermission DirectPermissionMask { get; private set; } = CompanyPermission.None;

    public DateTime JoinedAt { get; private set; }

    private CompanyMember() { }

    private CompanyMember(Guid companyId, Guid userId, Guid primaryPositionId)
    {
        if (companyId == Guid.Empty)
            throw new CompanyException("CompanyId inválido.");

        if (userId == Guid.Empty)
            throw new CompanyException("UserId inválido.");

        if (primaryPositionId == Guid.Empty)
            throw new CompanyException("PrimaryPositionId inválido.");

        CompanyId = companyId;
        UserId = userId;
        PrimaryPositionId = primaryPositionId;
        JoinedAt = DateTime.UtcNow;
    }

    internal static CompanyMember CreateFounder(Guid companyId, Guid userId, CompanyPosition position)
    {
        ValidatePosition(companyId, position);

        var member = new CompanyMember(companyId, userId, position.Id)
        {
            PrimaryPosition = position,
            Role = CompanyRole.Owner
        };

        return member;
    }

    internal static CompanyMember CreateMember(Guid companyId, Guid userId, CompanyRole role, CompanyPosition position)
    {
        if (role == CompanyRole.Owner)
            throw new CompanyException("Método inválido para criação de Owner.");

        ValidatePosition(companyId, position);

        var member = new CompanyMember(companyId, userId, position.Id)
        {
            PrimaryPosition = position,
            Role = role
        };

        return member;
    }

    private static void ValidatePosition(Guid companyId, CompanyPosition position)
    {
        if (position.CompanyId != companyId)
            throw new CompanyException("Cargo não pertence a esta empresa.");

        if (!position.IsActive)
            throw new CompanyException("Não é possível atribuir cargo inativo.");
    }

    public (bool changed, bool requiresSessionRefresh) ChangeRole(CompanyRole newRole)
        => SetRole(newRole, allowOwnerLevel: false);

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
        ValidatePosition(CompanyId, position);

        PrimaryPositionId = position.Id;
        PrimaryPosition = position;
    }
}