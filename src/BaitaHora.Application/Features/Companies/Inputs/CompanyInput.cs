using BaitaHora.Application.Addresses.DTOs.Inputs;

namespace BaitaHora.Application.Companies.Inputs;

public sealed record CompanyInput(
    string CompanyName,
    string Cnpj,
    string? TradeName,
    string Phone,
    string Email,
    AddressInput Address
);