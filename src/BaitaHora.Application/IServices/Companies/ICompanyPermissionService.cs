using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.IServices.Auth;

public interface ICompanyPermissionService
{
    Task<bool> CanAsync(Guid companyId, Guid userId, CompanyPermission permission, CancellationToken ct);
}