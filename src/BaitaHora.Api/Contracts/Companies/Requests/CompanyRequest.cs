namespace BaitaHora.Api.Contracts.Companies.Requests;

public sealed record CompanyRequest(
    string CompanyName,
    string Cnpj,
    string? TradeName,
    string CompanyPhone,
    string CompanyEmail,
    AddressRequest Address
);