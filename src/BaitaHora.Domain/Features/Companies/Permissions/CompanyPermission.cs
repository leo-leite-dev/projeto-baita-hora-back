namespace BaitaHora.Domain.Permissions
{
    [Flags]
    public enum CompanyPermission : long
    {
        None = 0,
        ManageMember = 1 << 0,
        RemoveMember = 1 << 1,
        ManageRoles = 1 << 2,
        ManagePositions = 1 << 3,
        ManageCompany = 1 << 4,

        All = ManageMember
            | RemoveMember
            | ManageRoles
            | ManagePositions
            | ManageCompany
    }
}