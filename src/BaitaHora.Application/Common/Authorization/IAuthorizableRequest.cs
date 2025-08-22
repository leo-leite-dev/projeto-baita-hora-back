using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.Common.Authorization;

public interface IAuthorizableRequest
{
    Guid ResourceId { get; }                  
    IEnumerable<CompanyPermission> RequiredPermissions { get; }
}