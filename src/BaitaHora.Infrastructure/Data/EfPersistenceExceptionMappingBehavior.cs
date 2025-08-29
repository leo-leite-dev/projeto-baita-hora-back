using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BaitaHora.Application.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Infrastructure.Data.Behaviors;

public sealed class EfPersistenceExceptionMappingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly ILogger<EfPersistenceExceptionMappingBehavior<TReq, TRes>> _log;

    public EfPersistenceExceptionMappingBehavior(
        ILogger<EfPersistenceExceptionMappingBehavior<TReq, TRes>> log)
    {
        _log = log;
    }

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Loga cada entry envolvida na concorrência com o máximo de contexto útil
            foreach (var entry in ex.Entries)
            {
                var entityName = entry.Metadata.Name;
                var state = entry.State;

                var keys = entry.Properties
                    .Where(p => p.Metadata.IsKey())
                    .Select(p => $"{p.Metadata.Name}={p.CurrentValue}")
                    .ToArray();

                var tokens = entry.Metadata.GetProperties()
                    .Where(p => p.IsConcurrencyToken)
                    .Select(p => new
                    {
                        p.Name,
                        Original = entry.OriginalValues[p.Name],
                        Current = entry.CurrentValues[p.Name]
                    })
                    .ToList();

                var modifiedProps = entry.Properties
                    .Where(p => p.IsModified)
                    .Select(p => new
                    {
                        p.Metadata.Name,
                        Original = entry.OriginalValues[p.Metadata.Name],
                        Current = entry.CurrentValues[p.Metadata.Name]
                    })
                    .ToList();

                _log.LogError(ex,
                    "EF Concurrency: Entity={Entity} State={State} Keys=[{Keys}] Tokens={@Tokens} ModifiedProps={@ModifiedProps} Request={RequestType} Inner={Inner}",
                    entityName,
                    state,
                    string.Join(", ", keys),
                    tokens,
                    modifiedProps,
                    typeof(TReq).Name,
                    ex.InnerException?.Message ?? "<null>");
            }

            // Converter para Result/Result<T> se o handler usa esse contrato
            return MapToResultConflict("Conflito de concorrência: o recurso foi modificado por outro processo.");
        }
        catch (DbUpdateException ex)
        {
            // DbUpdateException geral (FK/UNIQUE/etc.)
            _log.LogError(ex,
                "EF Persistência: {Message}. Request={RequestType} Inner={Inner}",
                ex.Message,
                typeof(TReq).Name,
                ex.InnerException?.Message ?? "<null>");

            return MapToResultConflict("Erro de persistência no banco de dados.");
        }
    }

    private TRes MapToResultConflict(string message)
    {
        // Result<T>
        if (typeof(TRes).IsGenericType &&
            typeof(TRes).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var genericArg = typeof(TRes).GetGenericArguments()[0];
            var conflict = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == nameof(Result.Conflict) && m.IsGenericMethod);
            var make = conflict.MakeGenericMethod(genericArg);
            return (TRes)make.Invoke(null, new object?[] { default, message })!;
        }

        // Result (não genérico), se existir na sua API
        if (typeof(TRes) == typeof(Result))
        {
            return (TRes)(object)Result.Conflict(message);
        }

        // Se o handler não retorna Result, repropaga (middleware HTTP trata)
        throw new DbUpdateException(message);
    }
}
