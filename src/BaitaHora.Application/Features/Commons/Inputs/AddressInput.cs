namespace BaitaHora.Application.Features.Commons.Inputs;

public sealed record AddressInput(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string ZipCode
);