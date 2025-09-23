namespace BaitaHora.Contracts.DTOS.Adresses;

public sealed record PatchAddressRequest(
    string? Street,
    string? Number,
    string? Complement,
    string? Neighborhood,
    string? City,
    string? State,
    string? ZipCode
);