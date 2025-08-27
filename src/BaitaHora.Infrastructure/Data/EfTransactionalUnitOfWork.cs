using BaitaHora.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace BaitaHora.Infrastructure.Data;

public sealed class EfTransactionalUnitOfWork : ITransactionalUnitOfWork
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _currentTx;

    public EfTransactionalUnitOfWork(AppDbContext db) => _db = db;

    public bool HasActiveTransaction => _currentTx is not null;

    public async Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTx is not null)
            return new EfAppTransaction(_currentTx, onDispose: () => _currentTx = null);

        _currentTx = await _db.Database.BeginTransactionAsync(ct);
        return new EfAppTransaction(_currentTx, onDispose: () => _currentTx = null);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    private sealed class EfAppTransaction : IAppTransaction
    {
        private readonly IDbContextTransaction _tx;
        private readonly Action _onDispose;
        private bool _committedOrRolledBack;

        public EfAppTransaction(IDbContextTransaction tx, Action onDispose)
        {
            _tx = tx;
            _onDispose = onDispose;
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_committedOrRolledBack) return;
            await _tx.CommitAsync(ct);
            _committedOrRolledBack = true;
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_committedOrRolledBack) return;
            await _tx.RollbackAsync(ct);
            _committedOrRolledBack = true;
        }

        public ValueTask DisposeAsync()
        {
            _tx.Dispose();
            _onDispose();
            return ValueTask.CompletedTask;
        }
    }
}