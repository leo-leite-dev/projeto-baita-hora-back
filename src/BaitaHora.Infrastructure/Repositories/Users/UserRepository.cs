using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using BaitaHora.Infrastructure.Commons.Repositories;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Users;

public sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public Task<User?> GetByEmailAsync(Email email, CancellationToken ct) =>
        _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> GetByUsernameAsync(Username username, CancellationToken ct) =>
        _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<bool> IsUserEmailTakenAsync(Email email, Guid? excludingUserId, CancellationToken ct)
    {
        var q = _context.Set<User>()
            .AsNoTracking()
            .Where(u => u.Email == email);

        if (excludingUserId.HasValue)
            q = q.Where(u => u.Id != excludingUserId.Value);

        return q.AnyAsync(ct);
    }

    public Task<bool> IsUsernameTakenAsync(Username username, Guid? excludingUserId, CancellationToken ct)
    {
        var q = _context.Set<User>()
            .AsNoTracking()
            .Where(u => u.Username == username);

        if (excludingUserId.HasValue)
            q = q.Where(u => u.Id != excludingUserId.Value);

        return q.AnyAsync(ct);
    }
}