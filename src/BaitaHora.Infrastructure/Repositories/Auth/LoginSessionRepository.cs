using BaitaHora.Application.IRepositories.Auth;
using BaitaHora.Application.IServices.Auth.Models;
using BaitaHora.Infrastructure.Data;
using BaitaHora.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Auth;

public sealed class LoginSessionRepository : ILoginSessionRepository
{
    private readonly AppDbContext _db;

    public LoginSessionRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(LoginSessionDto dto, CancellationToken ct)
    {
        var entity = new LoginSession
        {
            Id = dto.Id,
            UserId = dto.UserId,
            RefreshTokenHash = dto.RefreshTokenHash,
            RefreshTokenExpiresAtUtc = dto.RefreshTokenExpiresAtUtc,
            IsRevoked = dto.IsRevoked,
            Ip = dto.Ip,
            UserAgent = dto.UserAgent,
            CreatedAtUtc = dto.CreatedAtUtc
        };
        await _db.Set<LoginSession>().AddAsync(entity, ct);
    }

    public async Task<LoginSessionDto?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken ct)
    {
        var e = await _db.Set<LoginSession>()
                         .AsNoTracking()
                         .FirstOrDefaultAsync(x => x.RefreshTokenHash == refreshTokenHash, ct);

        return e is null ? null : new LoginSessionDto(
            e.Id, e.UserId, e.RefreshTokenHash, e.RefreshTokenExpiresAtUtc, e.IsRevoked, e.Ip, e.UserAgent, e.CreatedAtUtc);
    }

    public async Task InvalidateAsync(Guid sessionId, CancellationToken ct)
    {
        var e = await _db.Set<LoginSession>().FirstOrDefaultAsync(x => x.Id == sessionId, ct);
        if (e is null) return;
        e.IsRevoked = true;
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}