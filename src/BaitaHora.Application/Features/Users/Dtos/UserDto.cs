namespace BaitaHora.Application.Features.Users.Dtos;

public sealed record UserDto(
    string Username,
    string Email,
    UserProfileDto Profile
);