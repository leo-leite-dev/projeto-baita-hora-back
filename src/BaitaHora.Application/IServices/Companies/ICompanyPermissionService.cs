using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.IServices.Auth;

public interface ICompanyPermissionService
{
    Task<bool> CanAsync(Guid resourceId, Guid userId, CompanyPermission permission, CancellationToken ct);
}