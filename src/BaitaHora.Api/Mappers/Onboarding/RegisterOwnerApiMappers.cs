using BaitaHora.Application.Features.Onboarding;
using BaitaHora.Application.Features.Users.CreateUser;
using BaitaHora.Api.Mappers.Address;
using BaitaHora.Api.Mappers.Users;
using BaitaHora.Contracts.DTOs.Companies.Members;
using BaitaHora.Application.Features.Users.PatchUser;
using BaitaHora.Contracts.DTOS.Onbording;

namespace BaitaHora.Api.Mappers.Onboarding;

public static class RegisterOwnerApiMappers
{
    public static RegisterOwnerWithCompanyCommand ToCommand(this RegisterOwnerWithCompanyRequest r)
        => new RegisterOwnerWithCompanyCommand
        {
            Company = new CreateCompanyWithOwnerCommand
            {
                CompanyName = r.Company.CompanyName,
                Cnpj = r.Company.Cnpj,
                TradeName = r.Company.TradeName,
                CompanyEmail = r.Company.CompanyEmail,
                CompanyPhone = r.Company.CompanyPhone,
                Address = r.Company.Address.ToAddressCommand()
            },
            Owner = new CreateUserCommand(
                r.Owner.UserEmail,
                r.Owner.Username,
                r.Owner.RawPassword,
                r.Owner.Profile.ToUserProfileCommand()
            )
        };

    public static PatchOwnerCommand ToCommand(this PatchOwnerRequest r, Guid companyId)
        => new PatchOwnerCommand
        {
            CompanyId = companyId,
            NewOwner = r.Owner?.ToUserCommand() ?? new PatchUserCommand()
        };
}