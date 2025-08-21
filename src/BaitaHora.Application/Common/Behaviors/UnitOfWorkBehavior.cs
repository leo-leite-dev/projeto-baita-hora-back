using MediatR;

namespace BaitaHora.Application.Common.Behaviors;

public interface ITransactionalRequest { }

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

        await using var tx = await _uow.BeginTransactionAsync(ct);
        try
        {
            var response = await next();

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(tx, ct);

            return response;
        }
        catch
        {
            await _uow.RollbackTransactionAsync(tx, ct);
            throw;
        }
    }
}