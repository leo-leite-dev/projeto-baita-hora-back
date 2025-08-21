namespace BaitaHora.Application.Auth.Inputs;

public sealed record AuthenticateInput(
    string Identify,
    string Password,
    string? Ip = null,
    string? UserAgent = null
);