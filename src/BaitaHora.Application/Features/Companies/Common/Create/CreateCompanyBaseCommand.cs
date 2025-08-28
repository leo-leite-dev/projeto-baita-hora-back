using BaitaHora.Application.Features.Addresses.Create;

namespace BaitaHora.Application.Features.Companies.Common.Create;

public abstract record CreateCompanyBaseCommand
{
    public string CompanyName { get; init; } = default!;
    public string Cnpj { get; init; } = default!;
    public string? TradeName { get; init; }
    public string CompanyEmail { get; init; } = default!;
    public string CompanyPhone { get; init; } = default!;
    public CreateAddressCommand Address { get; init; } = default!;
}