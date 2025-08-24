using BaitaHora.Contracts.DTOS.Adress;

namespace BaitaHora.Contracts.DTOs.Companies;

public sealed record CreateCompanyRequest(
    string CompanyName,
    string Cnpj,
    string? TradeName,
    string CompanyPhone,
    string CompanyEmail,
    CreateAddressRequest Address
);