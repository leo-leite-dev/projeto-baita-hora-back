namespace BaitaHora.Domain.Permissions
{
    [Flags]
    public enum CompanyPermission
    {
        None = 0,
        AddMember = 1 << 0,
        RemoveMember = 1 << 1,
        EditMember = 1 << 2,
        ManageRoles = 1 << 3,
        ManagePositions = 1 << 4,
        EditCompany = 1 << 5,
    }
}