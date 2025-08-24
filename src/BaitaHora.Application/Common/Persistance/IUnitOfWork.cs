using Microsoft.EntityFrameworkCore.Storage;

namespace BaitaHora.Application.Common;

public interface IUnitOfWork
{
    bool HasActiveTransaction { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task CommitTransactionAsync(IDbContextTransaction tx, CancellationToken ct);
    Task RollbackTransactionAsync(IDbContextTransaction tx, CancellationToken ct);
}