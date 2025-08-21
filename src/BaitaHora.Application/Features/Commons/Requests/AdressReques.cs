namespace BaitaHora.Application.Features.Commons.Requests;

public sealed record AddressRequest(
    string Street,
    string Number,
    string Neighborhood,
    string City,
    string State,
    string ZipCode,
    string? Complement
);