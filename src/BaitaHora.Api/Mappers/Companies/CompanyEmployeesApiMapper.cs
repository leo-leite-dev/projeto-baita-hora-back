using BaitaHora.Api.Mappers.Users;
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
}