namespace BaitaHora.Application.Abstractions.Auth;

public sealed record JwtCookie(
    string Name,
    string Value,
    DateTimeOffset ExpiresAtUtc,
    bool HttpOnly,
    bool Secure,
    string Path,
    string SameSite
);