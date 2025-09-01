using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.Features.Companies.Guards.Interfaces
{
    public interface ICompanyGuards
    {
        Task<Result<Company>> EnsureCompanyExists(Guid companyId, CancellationToken ct);
        Task<Result<Company>> GetWithServiceOfferings(Guid companyId, CancellationToken ct);
        Task<Result<Company>> GetWithPositionsAndServiceOfferings(Guid companyId, CancellationToken ct);
        Task<Result<Company>> GetWithPositionsAndMembers(Guid companyId, CancellationToken ct);
        Task<Result<CompanyMember>> GetActiveMembership(Guid companyId, Guid userId, CancellationToken ct);
        Task<Result<bool>> HasPermissions(Guid companyId, Guid userId, IEnumerable<CompanyPermission> required, CancellationToken ct, bool requireAll = true);
        Task<Result<CompanyRole>> GetUserRole(Guid companyId, Guid userId, CancellationToken ct);
    }
}