using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Domain.Companies.Policies;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.Services;

public sealed class CompanyPermissionService : ICompanyPermissionService
{
    private readonly ICompanyMemberRepository _companyRepository;

    public CompanyPermissionService(ICompanyMemberRepository companyRepository) => _companyRepository = companyRepository;

    public async Task<bool> CanAsync(Guid companyId, Guid userId, CompanyPermission p, CancellationToken ct)
    {
        var (found, role, isActive) = await _companyRepository.GetRoleAsync(companyId, userId, ct);
        if (!found || !isActive) return false;

        return CompanyRolePolicies.Can(role, p);
    }
}