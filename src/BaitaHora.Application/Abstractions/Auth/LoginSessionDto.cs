namespace BaitaHora.Application.Abstractions.Auth;

public sealed record LoginSessionDto(
    Guid Id,
    Guid UserId,
    string RefreshTokenHash,
    DateTime RefreshTokenExpiresAtUtc,
    bool IsRevoked,
    string Ip,
    string UserAgent,
    DateTime CreatedAtUtc
);