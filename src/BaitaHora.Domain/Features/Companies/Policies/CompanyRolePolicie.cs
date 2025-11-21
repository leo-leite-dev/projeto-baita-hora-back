using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Domain.Permissions;

namespace BaitaHora.Domain.Companies.Policies;

public static class CompanyRolePolicies
{
    private static readonly CompanyRole[] Hierarchy =
    {
        CompanyRole.Owner,
        CompanyRole.Manager,
        CompanyRole.Staff,
        CompanyRole.Viewer
    };

    private static readonly IReadOnlyDictionary<CompanyRole, CompanyPermission> Map =
        new Dictionary<CompanyRole, CompanyPermission>
        {
            [CompanyRole.Owner] =
                CompanyPermission.All,

            [CompanyRole.Manager] =
                CompanyPermission.ManageMember
              | CompanyPermission.ManageCompany
              | CompanyPermission.EnableServiceOfferings
              | CompanyPermission.DisableServiceOfferings
              | CompanyPermission.DisablePositions
              | CompanyPermission.ManageSchedule,

            [CompanyRole.Staff] =
                CompanyPermission.ManageSchedule,

            [CompanyRole.Viewer] =
                CompanyPermission.None
        };

    public static int Rank(CompanyRole role)
    {
        var i = Array.IndexOf(Hierarchy, role);
        return i >= 0 ? i : int.MaxValue;
    }

    public static bool IsHigher(CompanyRole a, CompanyRole b) => Rank(a) < Rank(b);
    public static bool IsSameOrHigher(CompanyRole a, CompanyRole b) => Rank(a) <= Rank(b);

    public static CompanyPermission GetPermissions(CompanyRole role)
        => Map.TryGetValue(role, out var mask) ? mask : CompanyPermission.None;

    public static bool Has(CompanyRole role, CompanyPermission permission)
        => (GetPermissions(role) & permission) == permission;

    public static bool HasAny(CompanyRole role, IEnumerable<CompanyPermission> permissions)
        => permissions.Any(p => (GetPermissions(role) & p) == p);

    public static bool HasAny(CompanyRole role, params CompanyPermission[] permissions)
        => HasAny(role, (IEnumerable<CompanyPermission>)permissions);
}