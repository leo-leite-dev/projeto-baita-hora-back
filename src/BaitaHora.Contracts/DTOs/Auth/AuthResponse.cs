namespace BaitaHora.Contracts.DTOS.Auth;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Username,
    IReadOnlyList<string> Roles,
    IReadOnlyList<AuthCompanyResponse> Companies
);

public sealed record AuthCompanyResponse(
    Guid CompanyId,
    string Name
);