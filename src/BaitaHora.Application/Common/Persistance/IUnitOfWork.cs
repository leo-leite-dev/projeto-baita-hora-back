using Microsoft.EntityFrameworkCore.Storage;

namespace BaitaHora.Application.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(IDbContextTransaction tx, CancellationToken ct = default);
    Task RollbackTransactionAsync(IDbContextTransaction tx, CancellationToken ct = default);
}