namespace BaitaHora.Contracts.DTOS.Auth;

public sealed record AuthenticateRequest(
    string Identify,
    string Password,
    Guid CompanyId,
    string? Ip = null,
    string? UserAgent = null
);