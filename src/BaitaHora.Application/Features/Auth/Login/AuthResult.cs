using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Features.Auth.Login;

public sealed record AuthResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    Username Username,
    IReadOnlyList<CompanyRole> Roles,
    IReadOnlyList<AuthCompanySummary> Companies
);

public sealed record AuthCompanySummary(
    Guid CompanyId,
    string Name
);