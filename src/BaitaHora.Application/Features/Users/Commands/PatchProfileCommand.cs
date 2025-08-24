using BaitaHora.Application.Features.Commons.Commands;

namespace BaitaHora.Application.Features.Users.Commands;

public sealed record PatchUserProfileCommand(
    string? FullName = null,
    DateOnly? BirthDate = null,
    string? UserPhone = null,
    string? Cpf = null,
    string? Rg = null,
    PatchAddressCommand? Address = null
);