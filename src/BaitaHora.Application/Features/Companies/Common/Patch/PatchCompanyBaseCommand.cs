using BaitaHora.Application.Features.Addresses.PatchAddress;

namespace BaitaHora.Application.Features.Companies.Common.Patch;

public abstract record PatchCompanyBaseCommand : ICompanyLike
{
    public string? CompanyName { get; init; }
    public string? Cnpj { get; init; }
    public string? TradeName { get; init; }
    public string? CompanyEmail { get; init; }
    public string? CompanyPhone { get; init; }
    public PatchAddressCommand? Address { get; init; }
}