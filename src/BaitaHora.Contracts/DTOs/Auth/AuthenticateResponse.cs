namespace BaitaHora.Contracts.DTOS.Auth;

public sealed record AuthenticateResponse(
    bool IsAuthenticated,
    string? UserId,
    string? Username,
    string Role,
    string? CompanyId,
    string? MemberId
);