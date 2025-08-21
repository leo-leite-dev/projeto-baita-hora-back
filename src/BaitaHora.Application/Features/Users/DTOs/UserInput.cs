namespace BaitaHora.Application.Features.Users.DTOs;

public sealed record UserInput(
    string Email,
    string Username,
    string RawPassword,
    UserProfileInput Profile
);