using BaitaHora.Application.Features.Addresses.Common;

namespace BaitaHora.Application.Features.Addresses.PatchAddress;

public sealed record PatchAddressCommand : IAddressLike
{
    public string? Street { get; init; }
    public string? Number { get; init; }
    public string? Complement { get; init; }
    public string? Neighborhood { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }
}