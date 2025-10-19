namespace BaitaHora.Contracts.DTOs.Users;

public sealed record PatchUserRequest(
    string? Email,
    string? Username,
    PatchUserProfileRequest? Profile
);