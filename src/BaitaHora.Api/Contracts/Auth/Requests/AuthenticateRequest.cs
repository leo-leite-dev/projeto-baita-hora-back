namespace BaitaHora.Api.Contracts.Auth.Requests;

public sealed record AuthenticateRequest(
    string Identify,
    string Password,
    string? Ip = null,
    string? UserAgent = null
);