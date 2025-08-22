namespace BaitaHora.Application.Features.Users.Commands;

public sealed record UserCommand(
    string UserEmail,
    string Username,
    string RawPassword,
    UserProfileCommand Profile
);