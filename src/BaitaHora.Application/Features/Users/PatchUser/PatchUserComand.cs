using BaitaHora.Application.Features.Users.PatchUserProfile;

namespace BaitaHora.Application.Features.Users.PatchUser;

public sealed record PatchUserCommand
{
    public string? NewUserEmail { get; init; }
    public string? NewUsername { get; init; }
    public PatchUserProfileCommand? NewProfile { get; init; }
}