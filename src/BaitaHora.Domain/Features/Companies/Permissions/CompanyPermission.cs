namespace BaitaHora.Domain.Permissions
{
    [Flags]
    public enum CompanyPermission
    {
        None = 0,

        ManageMember = 1 << 0,
        ManageRoles = 1 << 1,
        ManageCompany = 1 << 2,

        EnableServiceOfferings = 1 << 3,
        DisableServiceOfferings = 1 << 4,
        DisablePositions = 1 << 5,
        RemoveMember = 1 << 6,
        RemoveServiceOfferings = 1 << 7,
        RemovePositions = 1 << 8,

        All =
            ManageMember |
            ManageRoles |
            ManageCompany |
            EnableServiceOfferings |
            DisableServiceOfferings |
            DisablePositions |
            RemoveMember |
            RemoveServiceOfferings |
            RemovePositions
    }
}
