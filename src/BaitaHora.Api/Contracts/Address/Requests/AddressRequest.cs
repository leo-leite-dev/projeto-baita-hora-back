namespace BaitaHora.Api.Contracts.Companies.Requests;

public sealed record AddressRequest(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string ZipCode
);