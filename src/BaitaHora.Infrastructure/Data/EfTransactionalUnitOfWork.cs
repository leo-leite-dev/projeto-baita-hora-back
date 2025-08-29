// EfTransactionalUnitOfWork.cs
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaitaHora.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;                // <- precisa
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;                // <- precisa

namespace BaitaHora.Infrastructure.Data;

public sealed class EfTransactionalUnitOfWork : ITransactionalUnitOfWork
{
    private readonly AppDbContext _db;
    private readonly ILogger<EfTransactionalUnitOfWork> _log; // <- logger
    private IDbContextTransaction? _currentTx;

    public EfTransactionalUnitOfWork(AppDbContext db, ILogger<EfTransactionalUnitOfWork> log)
    {
        _db = db;
        _log = log;
    }

    public bool HasActiveTransaction => _currentTx is not null;

    public async Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTx is not null)
            return new EfAppTransaction(_currentTx, onDispose: () => _currentTx = null);

        _currentTx = await _db.Database.BeginTransactionAsync(ct);
        return new EfAppTransaction(_currentTx, onDispose: () => _currentTx = null);
    }

    // >>> Substitua o método por este <<<
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        var pending = _db.ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Unchanged)
            .Select(e => new
            {
                Entity = e.Entity.GetType().Name,
                State = e.State.ToString(),
                Keys = string.Join(",", e.Properties
                    .Where(p => p.Metadata.IsKey())
                    .Select(p => $"{p.Metadata.Name}={p.CurrentValue}")),
                ModifiedProps = e.Properties
                    .Where(p => p.IsModified)
                    .Select(p => p.Metadata.Name)
                    .ToArray()
            })
            .ToList();

        _log.LogInformation("UoW SaveChanges - Pending entries: {@Pending}", pending);

        try
        {
            var affected = await _db.SaveChangesAsync(ct);
            _log.LogInformation("UoW SaveChanges - Rows affected: {Affected}", affected);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                var keys = string.Join(",", entry.Properties
                    .Where(p => p.Metadata.IsKey())
                    .Select(p => $"{p.Metadata.Name}={p.CurrentValue}"));

                _log.LogError(ex,
                    "DbUpdateConcurrencyException em {Entity}. State={State}. Keys={Keys}",
                    entry.Metadata.Name, entry.State, keys);
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            _log.LogError(ex, "DbUpdateException durante SaveChanges.");
            throw;
        }
    }

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
