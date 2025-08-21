using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.IRepositories.Companies;

public interface ICompanyMemberRepository
{
    Task<CompanyMember?> GetAsync(Guid companyId, Guid userId, CancellationToken ct);
    Task<(bool found, CompanyRole role, bool isActive)> GetRoleAsync(Guid companyId, Guid userId, CancellationToken ct);
}