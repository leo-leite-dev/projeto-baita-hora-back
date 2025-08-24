namespace BaitaHora.Contracts.DTOs.Users;

public sealed record PatchUserRequest(
    string? UserEmail,
    string? Username,
    PatchUserProfileRequest? Profile
);