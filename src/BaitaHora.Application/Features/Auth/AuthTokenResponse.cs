using BaitaHora.Application.Abstractions.Companies;

namespace BaitaHora.Application.Features.Auth;

public sealed record AuthTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Username,
    IEnumerable<string> Roles,
    IEnumerable<CompanySummary> Companies
);