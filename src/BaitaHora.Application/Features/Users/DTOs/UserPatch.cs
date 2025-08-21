namespace BaitaHora.Application.Features.Users.DTOs;

public sealed record UserPatch(
    string? Email,
    string? Username,
    UserProfilePatch? Profile
);