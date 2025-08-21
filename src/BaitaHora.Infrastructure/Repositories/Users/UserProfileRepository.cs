using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using BaitaHora.Infrastructure.Commons.Repositories;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Users;

public sealed class UserProfileRepository : GenericRepository<User>, IUserProfileRepository
{
    public UserProfileRepository(AppDbContext context) : base(context) { }

    public Task<bool> IsCpfTakenAsync(CPF cpf, Guid? excludingUserId, CancellationToken ct)
    {
        var q = _context.Set<UserProfile>()
            .AsNoTracking()
            .Where(up => up.Cpf == cpf);

        if (excludingUserId.HasValue)
            q = q.Where(u => u.Id != excludingUserId.Value);

        return q.AnyAsync(ct);
    }

    public Task<bool> IsRgTakenAsync(RG rg, Guid? excludingUserId, CancellationToken ct)
    {
        var q = _context.Set<UserProfile>()
            .AsNoTracking()
            .Where(up => up.Rg == rg);

        if (excludingUserId.HasValue)
            q = q.Where(u => u.Id != excludingUserId.Value);

        return q.AnyAsync(ct);
    }
}