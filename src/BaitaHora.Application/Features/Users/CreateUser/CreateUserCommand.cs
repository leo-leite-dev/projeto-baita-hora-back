using BaitaHora.Application.Features.Users.CreateUserProfile;

namespace BaitaHora.Application.Features.Users.CreateUser;

public sealed record CreateUserCommand(
    string UserEmail,
    string Username,
    string RawPassword,
    CreateUserProfileCommand Profile
);