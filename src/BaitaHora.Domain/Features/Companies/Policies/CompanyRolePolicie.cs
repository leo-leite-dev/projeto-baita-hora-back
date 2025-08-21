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

    public static int Rank(CompanyRole role)
    {
        var i = Array.IndexOf(Hierarchy, role);
        return i >= 0 ? i : int.MaxValue;
    }

    public static bool IsHigher(CompanyRole a, CompanyRole b) => Rank(a) < Rank(b);
    public static bool IsSameOrHigher(CompanyRole a, CompanyRole b) => Rank(a) <= Rank(b);

    public static CompanyPermission GetPermissions(CompanyRole role) => role switch
    {
        CompanyRole.Owner =>
            CompanyPermission.AddMember
          | CompanyPermission.RemoveMember
          | CompanyPermission.EditMember
          | CompanyPermission.ManageRoles
          | CompanyPermission.ManagePositions
          | CompanyPermission.EditCompany,

        CompanyRole.Manager =>
            CompanyPermission.AddMember
          | CompanyPermission.RemoveMember
          | CompanyPermission.EditMember
          | CompanyPermission.ManagePositions,

        _ => CompanyPermission.None
    };

    public static bool Can(CompanyRole role, CompanyPermission permission) =>
        (GetPermissions(role) & permission) == permission;
}