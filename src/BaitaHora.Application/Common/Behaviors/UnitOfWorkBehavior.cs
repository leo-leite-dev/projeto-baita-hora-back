using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace BaitaHora.Application.Common.Behaviors;

public interface ITransactionalRequest { }

public interface ITransactionalCommitCondition<in TResponse>
{
    bool ShouldCommit(TResponse response);
}

public sealed class UnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _uow;

    public UnitOfWorkBehavior(IUnitOfWork uow) => _uow = uow;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is not ITransactionalRequest)
            return await next();

        var ownsTx = !_uow.HasActiveTransaction;
        IDbContextTransaction? tx = null;

        try
        {
            if (ownsTx)
                tx = await _uow.BeginTransactionAsync(ct);

            var response = await next();

            bool shouldCommit =
                request is ITransactionalCommitCondition<TResponse> custom ? custom.ShouldCommit(response) :
                response is Result r ? r.IsSuccess :
                true;

            if (!shouldCommit)
            {
                if (ownsTx && tx is not null) await _uow.RollbackTransactionAsync(tx, ct);
                return response;
            }

            await _uow.SaveChangesAsync(ct);

            if (ownsTx && tx is not null)
                await _uow.CommitTransactionAsync(tx, ct);

            return response;
        }
        catch
        {
            if (ownsTx && tx is not null)
                await _uow.RollbackTransactionAsync(tx, ct);
            throw;
        }
        finally
        {
            if (ownsTx && tx is not null)
                await tx.DisposeAsync();
        }
    }
}