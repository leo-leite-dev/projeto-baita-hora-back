using BaitaHora.Domain.Features.Commons;
using BaitaHora.Domain.Features.Commons.Exceptions;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Features.Users.Entities;

namespace BaitaHora.Domain.Features.Companies.Entities;

public class CompanyMember : Entity
{
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public CompanyRole Role { get; private set; }

    public Guid? PrimaryPositionId { get; private set; }
    public CompanyPosition? PrimaryPosition { get; private set; }

    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;
    public bool IsActive { get; private set; } = true;

    protected CompanyMember() { }

    public static CompanyMember Create(Guid companyId, Guid userId, CompanyRole role)
    {
        var companyMember = new CompanyMember
        {
            CompanyId = companyId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow,
            IsActive = true
        };

        companyMember.SetRole(role);
        return companyMember;
    }

    public void PromoteTo(CompanyRole newRole)
    {
        Role = newRole;
    }

    public void SetRole(CompanyRole role)
    {
        Role = role;
    }

    public bool SetPrimaryPosition(CompanyPosition position)
    {
        if (position is null)
            throw new CompanyException("Posição é obrigatória.");

        if (!IsActive)
            throw new CompanyException("Não é possível alterar cargo de membro inativo.");

        if (!position.IsActive)
            throw new CompanyException("Não é possível atribuir uma posição inativa.");

        if (position.CompanyId != CompanyId)
            throw new CompanyException("A posição deve pertencer à mesma empresa.");

        if (position.AccessLevel == CompanyRole.Owner && Role != CompanyRole.Owner)
            throw new CompanyException("Cargo de nível Owner só pode ser atribuído ao fundador.");

        if (PrimaryPositionId == position.Id)
            return false;

        PrimaryPositionId = position.Id;

        PrimaryPosition = position;

        return true;
    }
}