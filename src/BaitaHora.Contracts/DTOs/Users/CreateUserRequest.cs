namespace BaitaHora.Contracts.DTOs.Users;

public sealed record CreateUserRequest(
    string UserEmail,
    string Username,
    string RawPassword,
    CreateUserProfileRequest Profile
);