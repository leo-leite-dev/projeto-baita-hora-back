using BaitaHora.Application.Features.Commons.Inputs;

namespace BaitaHora.Application.Features.Companies.Inputs;

public sealed record CompanyInput(
    string CompanyName,
    string Cnpj,
    string? TradeName,
    string Phone,
    string Email,
    AddressInput Address
);