using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.IRepositories.Auth;
using BaitaHora.Application.IServices.Auth.Models;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Infrastructure.Services.Auth;

public sealed class SessionService : ISessionService
{
    private readonly ILoginSessionRepository _sessions;
    private readonly ILogger<SessionService> _log;

    public SessionService(ILoginSessionRepository sessions, ILogger<SessionService> log)
    {
        _sessions = sessions;
        _log = log;
    }

    public async Task RegisterLoginAsync(Guid userId, string ip, string userAgent, CancellationToken ct)
    {
        var dto = new LoginSessionDto(
            Id: Guid.NewGuid(),
            UserId: userId,
            RefreshTokenHash: string.Empty, // será definido pelo TokenService quando emitir
            RefreshTokenExpiresAtUtc: DateTime.UtcNow, // idem
            IsRevoked: false,
            Ip: ip ?? string.Empty,
            UserAgent: userAgent ?? string.Empty,
            CreatedAtUtc: DateTime.UtcNow
        );

        // Muitas equipes registram o login em tabela separada (audit log).
        // Aqui estamos usando a mesma entidade por simplicidade. Em prod, recomendo tabela AuditLogin.
        await _sessions.AddAsync(dto, ct);
        await _sessions.SaveChangesAsync(ct);
        _log.LogInformation("Login registrado. userId={UserId} ip={Ip}", userId, ip);
    }
}
