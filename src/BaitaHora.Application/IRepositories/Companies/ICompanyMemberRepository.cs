using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.IRepositories.Companies
{
    public interface ICompanyMemberRepository : IGenericRepository<CompanyMember>
    {
        Task<bool> HasAnyMembershipAsync(Guid userId, CancellationToken ct);
        Task<bool> IsMemberOfCompanyAsync(Guid companyId, Guid userId, CancellationToken ct);
        Task<CompanyMember?> GetMemberAsync(Guid companyId, Guid userId, CancellationToken ct);

        Task<IReadOnlyList<CompanyMember>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<CompanyMember?> GetByCompanyAndUserWithPositionAsync(Guid companyId, Guid userId, CancellationToken ct = default);
        Task<(bool found, CompanyRole role, bool isActive)> GetRoleAsync(Guid companyId, Guid userId, CancellationToken ct);
    }
}