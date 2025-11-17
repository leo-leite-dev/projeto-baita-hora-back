using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using BaitaHora.Application.Common.Errors;

namespace BaitaHora.Infrastructure.Common.Behaviors;

public sealed class EfNpgsqlExceptionTranslatorBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
{
    private readonly IDbErrorTranslator _translator;
    public EfNpgsqlExceptionTranslatorBehavior(IDbErrorTranslator translator) => _translator = translator;

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg)
        {
            switch (pg.SqlState)
            {
                case PostgresErrorCodes.UniqueViolation:
                    var msgU = _translator.TryTranslateUniqueViolation(pg.ConstraintName, pg.Detail)
                               ?? "Violação de unicidade.";
                    throw new UniqueConstraintViolationException(msgU, pg.ConstraintName, ex);

                case PostgresErrorCodes.ForeignKeyViolation:
                    throw new ForeignKeyConstraintViolationException("Violação de integridade referencial.", pg.ConstraintName, ex);

                default:
                    throw;
            }
        }
    }
}