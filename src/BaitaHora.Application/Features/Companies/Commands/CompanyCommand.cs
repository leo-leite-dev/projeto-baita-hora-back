using BaitaHora.Application.Features.Commons.Commands;

namespace BaitaHora.Application.Features.Companies.Commands;

public sealed record CompanyCommand(
    string CompanyName,
    string Cnpj,
    string? TradeName,
    string CompanyEmail,
    string CompanyPhone,
    AddressCommand Address
);