using BaitaHora.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace BaitaHora.Infrastructure.Data;

internal sealed class EfAppTransaction : IAppTransaction
{
    private readonly IDbContextTransaction _tx;
    private readonly bool _owns;
    private readonly Action? _onDispose;

    public EfAppTransaction(IDbContextTransaction tx, bool ownsTransaction, Action? onDispose = null)
    {
        _tx = tx;
        _owns = ownsTransaction;
        _onDispose = onDispose;
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_owns)
            await _tx.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_owns)
            await _tx.RollbackAsync(ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_owns)
            await _tx.DisposeAsync();
        _onDispose?.Invoke();
    }
}