namespace BaitaHora.Api.Contracts.Auth.Mappers;

using BaitaHora.Api.Contracts.Auth.Requests;
using BaitaHora.Application.Features.Auth.Commands;
using BaitaHora.Application.Features.Commons.Commands;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Users.Commands;

public static class RegisterOwnerApiMappers
{
    public static RegisterOwnerWithCompanyCommand ToCommand(this RegisterOwnerWithCompanyRequest r)
        => new()
        {
            Company = new CompanyCommand(
                r.Company.CompanyName,
                r.Company.Cnpj,
                r.Company.TradeName,
                r.Company.CompanyEmail,
                r.Company.CompanyPhone,
                new AddressCommand(
                    r.Company.Address.Street,
                    r.Company.Address.Number,
                    r.Company.Address.Complement,
                    r.Company.Address.Neighborhood,
                    r.Company.Address.City,
                    r.Company.Address.State,
                    r.Company.Address.ZipCode
                )
            ),
            Owner = new UserCommand(
                r.Owner.UserEmail,
                r.Owner.Username,
                r.Owner.RawPassword,
                new UserProfileCommand(
                    r.Owner.Profile.FullName,
                    r.Owner.Profile.BirthDate,
                    r.Owner.Profile.UserPhone,
                    r.Owner.Profile.Cpf,
                    r.Owner.Profile.Rg,
                    new AddressCommand(
                        r.Owner.Profile.Address.Street,
                        r.Owner.Profile.Address.Number,
                        r.Owner.Profile.Address.Complement,
                        r.Owner.Profile.Address.Neighborhood,
                        r.Owner.Profile.Address.City,
                        r.Owner.Profile.Address.State,
                        r.Owner.Profile.Address.ZipCode
                    )
                )
            )
        };
}