namespace BaitaHora.Application.Auth.DTOs.Responses;

public sealed record AuthTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Username,
    IEnumerable<string> Roles
);