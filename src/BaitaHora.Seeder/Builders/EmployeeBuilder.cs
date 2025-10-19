using BaitaHora.Contracts.DTOs.Users;

namespace BaitaHora.Seeder.Builders;

public static class EmployeeBuilder
{
    public static CreateUserRequest Default()
    {
        var profile = new CreateUserProfileRequest(
            FullName: "Leoker",
            Phone: "+55 51 98888-5555",
            BirthDate: new DateTime(1990, 1, 10),
            Cpf: "18728901053",
            Rg: "123456780",
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
            Email: "leoker@baitahora.com",
            Username: "Leoker",
            RawPassword: "SenhaForte@123",
            Profile: profile
        );
    }
}