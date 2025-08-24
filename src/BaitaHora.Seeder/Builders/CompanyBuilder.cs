using BaitaHora.Contracts.DTOs.Companies;

namespace BaitaHora.Seeder.Builders;

public static class CompanyBuilder
{
    public static CreateCompanyRequest Default()
        => new(
            CompanyName: "BaitaHora LTDA",
            Cnpj: "12345678000199",
            TradeName: "BaitaHora",
            CompanyPhone: "+55 51 99999-9999",
            CompanyEmail: "contato@baitahora.com",
            Address: AddressBuilder.Build(
                street: "Rua X",
                number: "123",
                neighborhood: "Centro",
                city: "Porto Alegre",
                state: "RS",
                zip: "93228170"
            )
        );
}