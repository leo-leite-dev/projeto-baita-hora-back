namespace BaitaHora.Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string UserEmail,
    string Username,
    string RawPassword,
    CreateUserProfileCommand Profile
);