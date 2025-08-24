using BaitaHora.Contracts.DTOs.Users;
using BaitaHora.Contracts.DTOS.Adress;

namespace BaitaHora.Seeder.Builders;

public static class OwnerBuilder
{
    public static CreateUserRequest Default()
    {
        var profile = new CreateUserProfileRequest(
            FullName: "Dono da Firma",
            UserPhone: "+55 51 98888-7777",
            BirthDate: new DateTime(1990, 1, 10),
            Cpf: "11122233344",
            Rg: "123456789",
            Address: AddressBuilder.Build(
                street: "Rua X",
                number: "123",
                neighborhood: "Centro",
                city: "Porto Alegre",
                state: "RS",
                zip: "90000-000"
            )
        );

        return new CreateUserRequest(
            UserEmail: "owner@baitahora.com",
            Username: "owner",
            RawPassword: "123456",
            Profile: profile
        );
    }
}