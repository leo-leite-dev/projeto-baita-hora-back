namespace BaitaHora.Application.Users.DTOs;

public sealed record UserRequest(
    string Email,
    string Username,
    string Password,
    UserProfileRequest ProfileRequest
);