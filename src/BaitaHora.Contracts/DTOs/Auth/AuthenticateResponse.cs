namespace BaitaHora.Contracts.DTOS.Auth;

public sealed record AuthenticateResponse(
    bool IsAuthenticated,
    string? UserId,
    string? Username,
    IEnumerable<string> Roles,
    string? CompanyId,
    string? MemberId
);