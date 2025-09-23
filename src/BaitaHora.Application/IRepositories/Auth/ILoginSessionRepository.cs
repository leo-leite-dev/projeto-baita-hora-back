using BaitaHora.Application.Abstractions.Auth;

namespace BaitaHora.Application.IRepositories.Auth;

public interface ILoginSessionRepository
{
    Task AddAsync(LoginSessionDto session, CancellationToken ct);
    Task<LoginSessionDto?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct);
    Task InvalidateAsync(Guid sessionId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}