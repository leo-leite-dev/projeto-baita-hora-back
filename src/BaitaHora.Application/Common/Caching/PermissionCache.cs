using BaitaHora.Domain.Permissions;

namespace BaitaHora.Application.Common.Caching;

public sealed class PermissionCache
{
    private readonly Dictionary<(Guid companyId, Guid userId), CompanyPermission?> _masks = new();

    public bool TryGet(Guid companyId, Guid userId, out CompanyPermission? mask)
        => _masks.TryGetValue((companyId, userId), out mask);

    public void Set(Guid companyId, Guid userId, CompanyPermission? mask)
        => _masks[(companyId, userId)] = mask;
}