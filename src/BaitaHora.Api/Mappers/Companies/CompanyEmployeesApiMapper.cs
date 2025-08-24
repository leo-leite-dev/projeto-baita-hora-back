using BaitaHora.Application.Features.Commons.Commands;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Users.Commands;
using BaitaHora.Contracts.DTOs.Companies;
using BaitaHora.Contracts.DTOs.Users;
using BaitaHora.Contracts.DTOS.Adress;

namespace BaitaHora.Api.Mappers.Companies;

public static class CompanyEmployeesApiMapper
{
    public static CreateAddressCommand ToCommand(this CreateAddressRequest a)
        => new(a.Street, a.Number, a.Complement, a.Neighborhood, a.City, a.State, a.ZipCode);

    public static CreateUserProfileCommand ToCommand(this CreateUserProfileRequest p)
        => new(p.FullName, p.BirthDate, p.UserPhone, p.Cpf, p.Rg, p.Address.ToCommand());

    public static CreateUserCommand ToUserCommand(this RegisterEmployeeRequest r)
        => new(r.Employee.UserEmail, r.Employee.Username, r.Employee.RawPassword, r.Employee.Profile.ToCommand());

    public static RegisterEmployeeCommand ToCommand(this RegisterEmployeeRequest r, Guid companyId)
        => new()
        {
            CompanyId = companyId,
            PositionId = r.PositionId,
            Employee = r.ToUserCommand(),
        };


    public static PatchEmployeeCommand ToCommand(
        this PatchEmployeeRequest r, Guid companyId, Guid employeeId)
        => new()
        {
            CompanyId = companyId,
            EmployeeId = employeeId,
            PositionId = r.PositionId,

            Employee = r.Employee is null ? null : new PatchUserCommand
            {
                UserEmail = r.Employee.UserEmail,
                Username = r.Employee.Username,
                Profile = r.Employee.Profile is null ? null : new PatchUserProfileCommand
                {
                    FullName = r.Employee.Profile.FullName,
                    BirthDate = r.Employee.Profile.BirthDate,
                    UserPhone = r.Employee.Profile.UserPhone,
                    Cpf = r.Employee.Profile.Cpf,
                    Rg = r.Employee.Profile.Rg,
                    Address = r.Employee.Profile.Address is null ? null : new PatchAddressCommand
                    {
                        Street = r.Employee.Profile.Address.Street,
                        Number = r.Employee.Profile.Address.Number,
                        Complement = r.Employee.Profile.Address.Complement,
                        Neighborhood = r.Employee.Profile.Address.Neighborhood,
                        City = r.Employee.Profile.Address.City,
                        State = r.Employee.Profile.Address.State,
                        ZipCode = r.Employee.Profile.Address.ZipCode,
                    }
                }
            }
        };
}