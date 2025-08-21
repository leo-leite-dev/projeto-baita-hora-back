using BaitaHora.Domain.Commons;
using BaitaHora.Domain.Enums;
using BaitaHora.Domain.Users.Entities;

namespace BaitaHora.Domain.Entities.Companies
{
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

        public CompanyMember(Guid companyId, Guid userId, CompanyRole role, DateTime joinedAt, bool isActive)
        {
            CompanyId = companyId;
            UserId = userId;
            Role = role;
            JoinedAt = joinedAt;
            IsActive = isActive;
        }

        public static CompanyMember Create(Guid companyId, Guid userId, CompanyRole role, DateTime? joinedAtUtc = null, bool allowOwnerRole = false)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.", nameof(companyId));
            if (userId == Guid.Empty) throw new ArgumentException("UserId inválido.", nameof(userId));
            if (!Enum.IsDefined(typeof(CompanyRole), role)) throw new ArgumentException("Role inválido.", nameof(role));

            if (role == CompanyRole.Owner && !allowOwnerRole)
                throw new InvalidOperationException("Membro com nível Owner só pode ser criado com permissão explícita.");

            return new CompanyMember(
                companyId: companyId,
                userId: userId,
                role: role,
                joinedAt: joinedAtUtc ?? DateTime.UtcNow,
                isActive: true
            );
        }

        public void Activate() => IsActive = true;

        public void SetRole(CompanyRole role) => Role = role;

        public void SetPrimaryPosition(CompanyPosition pos)
        {
            if (pos is null) throw new ArgumentNullException(nameof(pos));
            if (!IsActive) throw new InvalidOperationException("Membro inativo.");
            if (!pos.IsActive) throw new InvalidOperationException("Cargo inativo.");
            if (pos.CompanyId != CompanyId) throw new InvalidOperationException("Cargo de outra empresa.");

            if (pos.AccessLevel == CompanyRole.Owner && Role != CompanyRole.Owner)
                throw new InvalidOperationException("Cargo de nível Owner só pode ser atribuído a membros Owner.");

            if (PrimaryPositionId == pos.Id) return;

            PrimaryPositionId = pos.Id;
            PrimaryPosition = pos;
        }

        public void Deactivate() => IsActive = false;

        public CompanyRole EffectiveLevel =>
            PrimaryPosition is null
                ? Role
                : Rank(PrimaryPosition.AccessLevel) > Rank(Role) ? PrimaryPosition.AccessLevel : Role;

        public static int Rank(CompanyRole r) => r switch
        {
            CompanyRole.Owner => 4,
            CompanyRole.Manager => 3,
            CompanyRole.Staff => 2,
            CompanyRole.Viewer => 1,
            _ => 0
        };
    }
}