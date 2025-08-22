using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.IRepositories.Companies
{
    public interface ICompanyMemberRepository : IGenericRepository<CompanyMember>
    {
        Task<(bool found, CompanyRole role, bool isActive)> GetRoleAsync(Guid companyId, Guid userId, CancellationToken ct);
    }
}