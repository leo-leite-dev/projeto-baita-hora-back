namespace BaitaHora.Application.Features.Auth.DTOs.Responses;

public sealed record AuthTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Username,
    IEnumerable<string> Roles
);