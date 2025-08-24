namespace BaitaHora.Application.Features.Commons.Commands;

public sealed record CreateAddressCommand(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string ZipCode
);