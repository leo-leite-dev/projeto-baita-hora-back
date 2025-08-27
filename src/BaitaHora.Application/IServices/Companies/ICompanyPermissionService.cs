using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.IServices.Companies;

public interface ICompanyPermissionService
{
    Task<bool> CanAsync(Guid companyId, Guid userId, CompanyPermission required, CancellationToken ct);

    Task<bool> CanAnyAsync(Guid companyId, Guid userId, IEnumerable<CompanyPermission> required, CancellationToken ct);

    Task<CompanyPermission?> GetMaskAsync(Guid companyId, Guid userId, CancellationToken ct);

    bool Has(CompanyPermission mask, CompanyPermission required);          // ALL por flag
    bool HasAny(CompanyPermission mask, IEnumerable<CompanyPermission> required);
}