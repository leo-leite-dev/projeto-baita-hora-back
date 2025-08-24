using BaitaHora.Application.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace BaitaHora.Infrastructure.Data;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public EfUnitOfWork(AppDbContext db) => _db = db;

    public bool HasActiveTransaction => _db.Database.CurrentTransaction is not null;

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        => _db.Database.BeginTransactionAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);

    public Task CommitTransactionAsync(IDbContextTransaction tx, CancellationToken ct = default)
        => tx.CommitAsync(ct);

    public Task RollbackTransactionAsync(IDbContextTransaction tx, CancellationToken ct = default)
        => tx.RollbackAsync(ct);
}
