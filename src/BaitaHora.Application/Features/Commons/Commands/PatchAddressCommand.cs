namespace BaitaHora.Application.Features.Commons.Commands;

public sealed record PatchAddressCommand(
    string? Street = null,
    string? Number = null,
    string? Complement = null,
    string? Neighborhood = null,
    string? City = null,
    string? State = null,
    string? ZipCode = null
);