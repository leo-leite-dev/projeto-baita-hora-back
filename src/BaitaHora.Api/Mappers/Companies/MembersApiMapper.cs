using BaitaHora.Api.Mappers.Users;
using BaitaHora.Application.Features.Companies.Employees.Activate;
using BaitaHora.Application.Features.Companies.Employees.Disable;
using BaitaHora.Application.Features.Companies.Members.ChangePosition;
using BaitaHora.Application.Features.Companies.Members.Employee;
using BaitaHora.Application.Features.Companies.Members.Employee.Register;
using BaitaHora.Application.Features.Users.PatchUser;
using BaitaHora.Application.Features.Users.PatchUserProfile;
using BaitaHora.Contracts.DTOs.Companies.Members;

namespace BaitaHora.Api.Mappers.Companies;

public static class MembersApiMapper
{
    public static RegisterMemberCommand ToCommand(this RegisterEmployeeRequest r)
        => new RegisterMemberCommand
        {
            PositionId = r.PositionId,
            Employee = r.ToUserCommand(),
        };

public static PatchEmployeeCommand ToCommand(this PatchMemberRequest r, Guid memberId)
    => new()
    {
        MemberId = memberId,
        NewMember = new PatchUserCommand
        {
            NewUserEmail = r.Email,
            NewProfile = new PatchUserProfileCommand { NewCpf = r.Cpf, NewRg = r.Rg }
        },
    };

    public static DisableMembersCommand ToCommand(this DisableEmployeesRequest r)
        => new DisableMembersCommand
        {
            EmployeeIds = (r?.EmployeeIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray(),
        };

    public static ActivateMembersCommand ToCommand(this ActivateEmployeesRequest r)
        => new ActivateMembersCommand
        {
            EmployeeIds = (r?.EmployeeIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray(),
        };

    public static ChangeMemberPositionCommand ToCommand(this ChangeMemberPositionRequest req, Guid userId)
        => new ChangeMemberPositionCommand(
            MemberId: userId,
            NewPositionId: req.PositionId,
            AlignRoleToPosition: req.AlignRoleToPosition
        );
}