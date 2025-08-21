namespace BaitaHora.Application.Addresses.DTOs.Inputs;

public sealed record AddressPatch(
    string? Street,
    string? Number,
    string? Complement,
    string? Neighborhood,
    string? City,
    string? State,
    string? ZipCode
);