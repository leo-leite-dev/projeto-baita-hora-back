using System.Security.Claims;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Infrastructure.Services.Auth.Security;

public static class PermissionClaimsExtensions
{
    private const string CompanyPermClaimType = "bh:company:perm";

    public static long? GetCompanyPermissionMask(this ClaimsPrincipal user, Guid companyId)
    {
        var claimType = $"{CompanyPermClaimType}:{companyId:D}";
        var claim = user.FindFirst(claimType);
        if (claim is null) return null;

        return long.TryParse(claim.Value, out var mask) ? mask : null;
    }

    public static bool HasPermission(this ClaimsPrincipal user, Guid companyId, CompanyPermission permission)
    {
        var mask = user.GetCompanyPermissionMask(companyId);
        if (mask is null) return false;

        var current = (CompanyPermission)mask.Value;
        return current.HasFlag(permission);
    }
}