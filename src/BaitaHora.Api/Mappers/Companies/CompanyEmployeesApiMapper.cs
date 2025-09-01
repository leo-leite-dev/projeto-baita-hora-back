using BaitaHora.Api.Mappers.Users;
using BaitaHora.Application.Features.Companies.Employees.Activate;
using BaitaHora.Application.Features.Companies.Employees.Disable;
using BaitaHora.Application.Features.Companies.Members.ChangePosition;
using BaitaHora.Application.Features.Companies.Members.Employee.Register;
using BaitaHora.Application.Features.Users.PatchUser;
using BaitaHora.Contracts.DTOs.Companies.Members;

namespace BaitaHora.Api.Mappers.Companies;

public static class CompanyEmployeesApiMapper
{
    public static RegisterEmployeeCommand ToCommand(
        this RegisterEmployeeRequest r, Guid companyId)
        => new RegisterEmployeeCommand
        {
            CompanyId = companyId,
            PositionId = r.PositionId,
            Employee = r.ToUserCommand(),
        };

    public static PatchEmployeeCommand ToCommand(
        this PatchEmployeeRequest r, Guid companyId, Guid employeeId)
        => new PatchEmployeeCommand
        {
            CompanyId = companyId,
            EmployeeId = employeeId,
            NewEmployee = r.Employee?.ToUserCommand() ?? new PatchUserCommand()
        };

    public static DisableEmployeesCommand ToCommand(
        this DisableEmployeesRequest r, Guid companyId)
        => new DisableEmployeesCommand
        {
            CompanyId = companyId,
            EmployeeIds = (r?.EmployeeIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };

    public static ActivateEmployeesCommand ToCommand(
        this ActivateEmployeesRequest r, Guid companyId)
        => new ActivateEmployeesCommand
        {
            CompanyId = companyId,
            EmployeeIds = (r?.EmployeeIds ?? Enumerable.Empty<Guid>())
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToArray()
        };

    public static ChangeMemberPositionCommand ToCommand(
        this ChangeMemberPositionRequest req, Guid companyId, Guid employeeId)
        => new(companyId, employeeId, req.PositionId, req.AlignRoleToPosition);
}