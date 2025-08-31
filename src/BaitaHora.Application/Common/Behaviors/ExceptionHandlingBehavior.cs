using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Common.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BaitaHora.Application.Common.Behaviors;

public sealed class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _log;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> log)
        => _log = log;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        try
        {
            return await next();
        }
        catch (ValidationException ex)
        {
            _log.LogWarning(ex, "Falha de validação em {RequestType}", typeof(TRequest).Name);
            return MapToGeneric<TResponse>(
                Result.BadRequest(ex.Message,
                    code: ResultCodes.Validation.Invalid,
                    title: "Pedido inválido"));
        }
        catch (CompanyException ex)
        {
            _log.LogWarning(ex, "Regra de negócio falhou em {RequestType}", typeof(TRequest).Name);

            var result = Result.Conflict(
                    message: ex.Message,
                    code: ResultCodes.Conflict.BusinessRule,
                    title: "Conflito de negócio"
                )
                .WithMeta("dominio", ex.Message);

            return MapToGeneric<TResponse>(result);
        }
        catch (ArgumentException ex)
        {
            _log.LogWarning(ex, "Argumento inválido em {RequestType}", typeof(TRequest).Name);
            return MapToGeneric<TResponse>(
                Result.BadRequest(ex.Message, title: "Parâmetro inválido"));
        }
        catch (KeyNotFoundException ex)
        {
            _log.LogWarning(ex, "Recurso não encontrado em {RequestType}", typeof(TRequest).Name);
            return MapToGeneric<TResponse>(
                Result.NotFound(ex.Message, title: "Recurso não encontrado"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _log.LogWarning(ex, "Acesso não autorizado em {RequestType}", typeof(TRequest).Name);
            return MapToGeneric<TResponse>(
                Result.Unauthorized(ex.Message, title: "Não autorizado"));
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Erro inesperado em {RequestType}", typeof(TRequest).Name);
            return MapToGeneric<TResponse>(
                Result.ServerError("Erro inesperado.", title: "Erro interno"));
        }
    }

    private static TResponse MapToGeneric<TResponse>(Result result)
    {
        var payloadType = typeof(TResponse).IsGenericType
            ? typeof(TResponse).GetGenericArguments()[0]
            : typeof(object);

        var generic = typeof(Result<>).MakeGenericType(payloadType);

        var fromError = generic.GetMethod(
            "FromError",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: new[] { typeof(Result) },
            modifiers: null
        ) ?? throw new MissingMethodException($"{generic.Name}.FromError(Result) não encontrado.");

        return (TResponse)fromError.Invoke(null, new object[] { result })!;
    }
}