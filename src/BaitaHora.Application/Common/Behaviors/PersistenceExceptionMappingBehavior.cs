using MediatR;
using BaitaHora.Application.Common.Errors;
using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class PersistenceExceptionMappingBehavior<TReq,TRes> : IPipelineBehavior<TReq,TRes>
{
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (UniqueConstraintViolationException ex)
        {
            throw new CompanyException(ex.Message, ex);
        }
        catch (ForeignKeyConstraintViolationException ex)
        {
            throw new CompanyException(ex.Message, ex);
        }
    }
}