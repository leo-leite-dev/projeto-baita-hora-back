namespace BaitaHora.Application.Features.Auth.Inputs;

public sealed record AuthenticateInput(
    string Identify,
    string Password,
    string? Ip = null,
    string? UserAgent = null
);