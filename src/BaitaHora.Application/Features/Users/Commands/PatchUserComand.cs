namespace BaitaHora.Application.Features.Users.Commands;

public sealed record PatchUserCommand(
    string? UserEmail = null,
    string? Username = null,
    PatchUserProfileCommand? Profile = null
);