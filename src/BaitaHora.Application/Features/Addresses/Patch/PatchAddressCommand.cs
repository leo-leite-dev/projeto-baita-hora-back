namespace BaitaHora.Application.Features.Addresses.PatchAddress;

public sealed record PatchAddressCommand : IAddressLike
{
    public string? NewStreet { get; init; }
    public string? NewNumber { get; init; }
    public string? NewComplement { get; init; }
    public string? NewNeighborhood { get; init; }
    public string? NewCity { get; init; }
    public string? NewState { get; init; }
    public string? NewZipCode { get; init; }

    string? IAddressLike.Street => NewStreet;
    string? IAddressLike.Number => NewNumber;
    string? IAddressLike.Complement => NewComplement;
    string? IAddressLike.Neighborhood => NewNeighborhood;
    string? IAddressLike.City => NewCity;
    string? IAddressLike.State => NewState;
    string? IAddressLike.ZipCode => NewZipCode;
}