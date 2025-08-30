using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces;

public interface ICompanyGuards
{
    Task<Result<Company>> ExistsCompany(Guid companyId, CancellationToken ct);                
    Task<Result<CompanyMember>> GetActiveMembershipOrForbiddenAsync(Guid companyId, Guid userId, CancellationToken ct);
    Task<Result<bool>> HasPermissionsOrForbiddenAsync(Guid companyId, Guid userId, IEnumerable<CompanyPermission> required, CancellationToken ct, bool requireAll = true);
    Task<Result<Company>> GetWithServiceOfferingsOrNotFoundAsync(Guid companyId, CancellationToken ct); 
}
