namespace BaitaHora.Application.Features.Addresses.Create;

public sealed record CreateAddressCommand : IAddressLike
{
    public string Street { get; init; } = default!;
    public string Number { get; init; } = default!;
    public string? Complement { get; init; }
    public string Neighborhood { get; init; } = default!;
    public string City { get; init; } = default!;
    public string State { get; init; } = default!;
    public string ZipCode { get; init; } = default!;

    string? IAddressLike.Street => Street;
    string? IAddressLike.Number => Number;
    string? IAddressLike.Complement => Complement;
    string? IAddressLike.Neighborhood => Neighborhood;
    string? IAddressLike.City => City;
    string? IAddressLike.State => State;
    string? IAddressLike.ZipCode => ZipCode;
}