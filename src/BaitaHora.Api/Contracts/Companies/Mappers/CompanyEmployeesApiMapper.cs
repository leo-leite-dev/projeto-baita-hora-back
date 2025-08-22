using BaitaHora.Api.Contracts.Auth.Requests;
using BaitaHora.Api.Contracts.Companies.Requests;
using BaitaHora.Api.Contracts.Users.Requests;
using BaitaHora.Application.Features.Commons.Commands;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Users.Commands;

namespace BaitaHora.Api.Contracts.Companies.Mappers;

public static class CompanyEmployeesApiMapper
{
    public static AddressCommand ToCommand(this AddressRequest a)
        => new(a.Street, a.Number, a.Complement, a.Neighborhood, a.City, a.State, a.ZipCode);

    public static UserProfileCommand ToCommand(this UserProfileRequest p)
        => new(p.FullName, p.BirthDate, p.UserPhone, p.Cpf, p.Rg, p.Address.ToCommand());

    public static UserCommand ToUserCommand(this RegisterEmployeeRequest r)
        => new(r.Employee.UserEmail, r.Employee.Username, r.Employee.RawPassword, r.Employee.Profile.ToCommand());

    public static RegisterEmployeeCommand ToCommand(this RegisterEmployeeRequest r, Guid companyId)
        => new()
        {
            CompanyId = companyId,
            PositionId = r.PositionId,
            Employee = r.ToUserCommand(),
        };
}