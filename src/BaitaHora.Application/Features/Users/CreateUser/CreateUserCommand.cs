using BaitaHora.Application.Features.Users.CreateUserProfile;

namespace BaitaHora.Application.Features.Users.CreateUser;

public sealed record CreateUserCommand
{
    public required string Email { get; init; }
    public required string Username { get; init; }
    public required string RawPassword { get; init; }
    public required CreateUserProfileCommand Profile { get; init; }
}