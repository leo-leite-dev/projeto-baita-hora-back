namespace BaitaHora.Application.Common.Persistence;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IAppTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}

public interface ITransactionalUnitOfWork : IUnitOfWork
{
    bool HasActiveTransaction { get; }
    Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct = default);
}