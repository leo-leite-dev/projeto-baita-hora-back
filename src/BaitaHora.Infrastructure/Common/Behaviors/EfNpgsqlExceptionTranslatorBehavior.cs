using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using BaitaHora.Application.Common.Errors;

namespace BaitaHora.Infrastructure.Common.Behaviors;

public sealed class EfNpgsqlExceptionTranslatorBehavior<TReq,TRes> : IPipelineBehavior<TReq,TRes>
{
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
                    throw new UniqueConstraintViolationException("Violação de unicidade.", pg.ConstraintName, ex);

                case PostgresErrorCodes.ForeignKeyViolation:
                    throw new ForeignKeyConstraintViolationException("Violação de integridade referencial.", pg.ConstraintName, ex);

                default:
                    throw; 
            }
        }
    }
}