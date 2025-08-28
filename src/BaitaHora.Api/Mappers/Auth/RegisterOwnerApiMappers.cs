using BaitaHora.Application.Features.Addresses.Create;
using BaitaHora.Contracts.DTOS.Auth;
using BaitaHora.Application.Features.Onboarding;
using BaitaHora.Application.Features.Users.CreateUser;
using BaitaHora.Application.Features.Users.CreateUserProfile;

namespace BaitaHora.Api.Mappers.Auth;

public static class RegisterOwnerApiMappers
{
    public static RegisterOwnerWithCompanyCommand ToCommand(this RegisterOwnerWithCompanyRequest r)
        => new()
        {
            Company = new CreateCompanyWithOwnerCommand
            {
                CompanyName = r.Company.CompanyName,
                Cnpj = r.Company.Cnpj,
                TradeName = r.Company.TradeName,
                CompanyEmail = r.Company.CompanyEmail,
                CompanyPhone = r.Company.CompanyPhone,
                Address = new CreateAddressCommand
                {
                    Street = r.Company.Address.Street,
                    Number = r.Company.Address.Number,
                    Complement = r.Company.Address.Complement,
                    Neighborhood = r.Company.Address.Neighborhood,
                    City = r.Company.Address.City,
                    State = r.Company.Address.State,
                    ZipCode = r.Company.Address.ZipCode
                }
            },
            Owner = new CreateUserCommand(
                r.Owner.UserEmail,
                r.Owner.Username,
                r.Owner.RawPassword,
                new CreateUserProfileCommand(
                    r.Owner.Profile.FullName,
                    r.Owner.Profile.BirthDate,
                    r.Owner.Profile.UserPhone,
                    r.Owner.Profile.Cpf,
                    r.Owner.Profile.Rg,
                    new CreateAddressCommand
                    {
                        Street = r.Owner.Profile.Address.Street,
                        Number = r.Owner.Profile.Address.Number,
                        Complement = r.Owner.Profile.Address.Complement,
                        Neighborhood = r.Owner.Profile.Address.Neighborhood,
                        City = r.Owner.Profile.Address.City,
                        State = r.Owner.Profile.Address.State,
                        ZipCode = r.Owner.Profile.Address.ZipCode
                    }
                )
            )
        };
}