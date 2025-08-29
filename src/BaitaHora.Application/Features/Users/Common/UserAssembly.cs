using BaitaHora.Application.Features.Users.CreateUser;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Application.Features.Users.Common;

public static class UserAssembler
{
    public readonly record struct OwnerVO(
        Email UserEmail,
        Username Username,
        string RawPassword,
        string FullName,
        CPF Cpf,
        RG? Rg,
        Phone UserPhone,
        DateOfBirth? BirthDate,
        Address Address
    );

    public static OwnerVO BuildOwnerVO(CreateUserCommand u)
    {
        var email = Email.Parse(u.UserEmail);
        var username = Username.Parse(u.Username);
        var cpf = CPF.Parse(u.Profile.Cpf);
        var phone = Phone.Parse(u.Profile.UserPhone);
        RG? rg = string.IsNullOrWhiteSpace(u.Profile.Rg) ? default : RG.Parse(u.Profile.Rg);

        var addr = Address.Parse(
            street: u.Profile.Address.Street,
            number: u.Profile.Address.Number,
            neighborhood: u.Profile.Address.Neighborhood,
            city: u.Profile.Address.City,
            state: u.Profile.Address.State,
            zipCode: u.Profile.Address.ZipCode,
            complement: u.Profile.Address.Complement
        );

        DateOfBirth? dob = u.Profile.BirthDate is DateOnly d
            ? DateOfBirth.Parse(d)
            : (DateOfBirth?)null;

        return new OwnerVO(
            UserEmail: email,
            Username: username,
            RawPassword: u.RawPassword,
            FullName: u.Profile.FullName,
            Cpf: cpf,
            Rg: rg,
            UserPhone: phone,
            BirthDate: dob,
            Address: addr
        );
    }
}
