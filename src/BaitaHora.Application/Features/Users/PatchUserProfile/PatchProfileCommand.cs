using BaitaHora.Application.Features.Addresses.PatchAddress;

namespace BaitaHora.Application.Features.Users.PatchUserProfile;

public sealed record PatchUserProfileCommand(
    string? FullName = null,
    DateOnly? BirthDate = null,
    string? UserPhone = null,
    string? Cpf = null,
    string? Rg = null,
    PatchAddressCommand? Address = null
);