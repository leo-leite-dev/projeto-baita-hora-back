using BaitaHora.Application.IRepositories.Users;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Users;

public sealed class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public Task<User?> GetByEmailAsync(Email email, CancellationToken ct) =>
        _context.Set<User>()
            .FirstOrDefaultAsync(u => u.UserEmail == email, ct);

    public Task<User?> GetByUsernameAsync(Username username, CancellationToken ct) =>
        _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<User?> GetByIdWithProfileAsync(Guid id, CancellationToken ct = default) =>
        _context.Set<User>()            
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
}