using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Features.Auth.Login;

public sealed record AuthResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    Username Username,
    CompanyRole Role,                         
    IReadOnlyList<AuthCompanySummary> Companies,
    Guid? MemberId = null
);

public sealed record AuthCompanySummary(
    Guid CompanyId,
    string Name
);
