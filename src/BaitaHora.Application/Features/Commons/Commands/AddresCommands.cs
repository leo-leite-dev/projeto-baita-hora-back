namespace BaitaHora.Application.Features.Commons.Commands;

public sealed record AddressCommand(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string ZipCode
);