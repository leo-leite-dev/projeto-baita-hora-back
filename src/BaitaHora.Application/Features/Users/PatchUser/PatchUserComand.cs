using BaitaHora.Application.Features.Users.PatchUserProfile;

namespace BaitaHora.Application.Features.Users.PatchUser;

public sealed record PatchUserCommand(
    string? UserEmail = null,
    string? Username = null,
    PatchUserProfileCommand? Profile = null
);