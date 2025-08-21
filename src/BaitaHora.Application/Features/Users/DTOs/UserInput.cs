namespace BaitaHora.Application.Features.Users.DTOs;

public sealed record UserInput(
    string UserEmail,
    string Username,
    string RawPassword,
    UserProfileInput Profile
);