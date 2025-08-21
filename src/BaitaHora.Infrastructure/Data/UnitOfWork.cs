using BaitaHora.Application.Common;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace BaitaHora.Infrastructure.Persistence
{
    public sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public EfUnitOfWork(AppDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
            => _db.Database.BeginTransactionAsync(ct);

        public Task CommitTransactionAsync(IDbContextTransaction tx, CancellationToken ct = default)
            => tx.CommitAsync(ct);

        public Task RollbackTransactionAsync(IDbContextTransaction tx, CancellationToken ct = default)
            => tx.RollbackAsync(ct);
    }
}