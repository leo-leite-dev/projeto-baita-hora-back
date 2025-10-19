using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Seeder.Builders;

public static class OwnerBuilder
{
    public static CreateUserRequest Default()
    {
        var profile = new CreateUserProfileRequest(
            FullName: "Dono da Firma",
            Phone: "+55 51 98888-7777",
            BirthDate: new DateTime(1990, 1, 10),
            Cpf: "03672513024",
            Rg: "123456789",
            Address: AddressBuilder.Build(
                street: "Rua X",
                number: "123",
                neighborhood: "Centro",
                city: "Porto Alegre",
                state: "RS",
                zip: "93228170"
            )
        );

        return new CreateUserRequest(
            Email: "owner@baitahora.com",
            Username: "owner",
            RawPassword: "SenhaForte@123",
            Profile: profile
        );
    }
}