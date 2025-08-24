using BaitaHora.Contracts.DTOS.Adress; // mantém "Adress" como no teu projeto

namespace BaitaHora.Seeder.Builders;

public static class AddressBuilder
{
    public static CreateAddressRequest Build(
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string zip,
        string? complement = null)
        => new(
            Street: street,
            Number: number,
            Complement: complement,
            Neighborhood: neighborhood,
            City: city,
            State: state,
            ZipCode: zip
        );
}