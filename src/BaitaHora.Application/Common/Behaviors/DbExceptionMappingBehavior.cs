using MediatR;
using Microsoft.EntityFrameworkCore;
using BaitaHora.Domain.Features.Commons.Exceptions;
using BaitaHora.Application.Common.Errors;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class DbExceptionMappingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IDbErrorTranslator _translator;

    public DbExceptionMappingBehavior(IDbErrorTranslator translator)
        => _translator = translator;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException pg)
        {
            if (pg.SqlState == Npgsql.PostgresErrorCodes.UniqueViolation)
            {
                var message = _translator.TryTranslateUniqueViolation(pg) ?? "Violação de unicidade.";
                throw new CompanyException(message, ex);
            }
            throw;
        }
    }
}