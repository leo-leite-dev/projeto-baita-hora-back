using BaitaHora.Application.Features.Commons.Inputs;

namespace BaitaHora.Application.Features.Companies.Inputs;

public sealed record CompanyInput(
    string CompanyName,
    string Cnpj,
    string? TradeName,
    string CompanyPhone,
    string CompanyEmail,
    AddressInput Address
);