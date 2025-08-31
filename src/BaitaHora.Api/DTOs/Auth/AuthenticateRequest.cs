namespace BaitaHora.Contracts.DTOS.Auth;

public sealed record AuthenticateRequest(
    Guid CompanyId,
    string Identify,
    string Password,
    string? Ip = null,
    string? UserAgent = null
);