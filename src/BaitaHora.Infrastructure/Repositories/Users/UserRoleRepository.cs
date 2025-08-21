// using BaitaHora.Application.IRepositories;
// using BaitaHora.Infrastructure.Data;

// namespace BaitaHora.Infrastructure.Repositories;

// public sealed class UserRoleRepository : IUserRoleRepository
// {
//     private readonly AppDbContext _db;
//     public UserRoleRepository(AppDbContext db) => _db = db;

//     public Task<IReadOnlyList<string>> GetRoleNamesByUserIdAsync(Guid userId, CancellationToken ct = default)
//         => _db.UserRoles
//               .Where(ur => ur.UserId == userId)
//               .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
//               .AsNoTracking()
//               .ToListAsync(ct)
//               .ContinueWith(t => (IReadOnlyList<string>)t.Result, ct);
// }
