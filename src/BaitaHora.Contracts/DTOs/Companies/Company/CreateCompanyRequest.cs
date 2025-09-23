using BaitaHora.Contracts.DTOS.Adresses;

namespace BaitaHora.Contracts.DTOs.Companies.Company.Create;

public sealed record CreateCompanyRequest(
    string CompanyName,
    string Cnpj,
    string? TradeName,
    string CompanyPhone,
    string CompanyEmail,
    CreateAddressRequest Address
);