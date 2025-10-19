namespace BaitaHora.Contracts.DTOs.Users;

public sealed record CreateUserRequest(
    string Email,
    string Username,
    string RawPassword,
    CreateUserProfileRequest Profile
);