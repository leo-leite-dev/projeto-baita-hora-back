namespace BaitaHora.Domain.Permissions
{
    [Flags]
    public enum CompanyPermission
    {
        None = 0,

        ManageMember = 1 << 0,
        RemoveMember = 1 << 5,

        ManageRoles = 1 << 1,

        ManageCompany = 1 << 2,

        EnableServiceOfferings = 1 << 3,
        DisableServiceOfferings = 1 << 4,
        RemoveServiceOfferings = 1 << 7,

        DisablePositions = 1 << 5,
        RemovePositions = 1 << 8,

        All =
            ManageMember |
            RemoveMember |
            ManageRoles |
            ManageCompany |
            DisableServiceOfferings |
            RemoveServiceOfferings |
            EnableServiceOfferings |
            DisablePositions |
            RemovePositions
    }
}