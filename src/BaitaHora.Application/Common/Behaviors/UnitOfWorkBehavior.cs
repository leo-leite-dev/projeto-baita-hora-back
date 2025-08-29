using BaitaHora.Application.Common.Persistence;
using BaitaHora.Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

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
    private readonly ITransactionalUnitOfWork _uow;
    private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _log;

    public UnitOfWorkBehavior(
        ITransactionalUnitOfWork uow,
        ILogger<UnitOfWorkBehavior<TRequest, TResponse>> log)
    {
        _uow = uow;
        _log = log;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (request is not ITransactionalRequest)
            return await next();

        var ownsTx = !_uow.HasActiveTransaction;
        IAppTransaction? tx = null;

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
                if (ownsTx && tx is not null)
                    await tx.RollbackAsync(ct);

                _log.LogInformation("UoW skip commit (ShouldCommit=false) para {RequestType}.",
                    typeof(TRequest).Name);
                return response;
            }

            await _uow.SaveChangesAsync(ct);

            if (ownsTx && tx is not null)
                await tx.CommitAsync(ct);

            _log.LogDebug("UoW commit concluído para {RequestType}.", typeof(TRequest).Name);
            return response;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Erro durante UoW. Request={RequestType}, HasActiveTx={HasTx}",
                typeof(TRequest).Name, _uow.HasActiveTransaction);

            if (ownsTx && tx is not null)
                await tx.RollbackAsync(ct);

            throw;
        }
        finally
        {
            if (ownsTx && tx is not null)
                await tx.DisposeAsync();
        }
    }
}