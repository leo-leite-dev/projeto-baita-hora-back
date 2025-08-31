using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using BaitaHora.Application.Common.Errors;

namespace BaitaHora.Api.Web.Middlewares;

public sealed class ExceptionHttpMappingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHttpMappingMiddleware> _logger;
    private readonly IDbErrorTranslator _dbTranslator;

    public ExceptionHttpMappingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHttpMappingMiddleware> logger,
        IDbErrorTranslator dbTranslator)
    {
        _next = next;
        _logger = logger;
        _dbTranslator = dbTranslator;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pex)
        {
            if (ctx.Response.HasStarted)
            {
                _logger.LogError(ex, "Erro DB após resposta iniciada. TraceId={TraceId}", ctx.TraceIdentifier);
                return;
            }

            if (pex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                var friendly = _dbTranslator.TryTranslateUniqueViolation(pex.ConstraintName, pex.Detail)
                               ?? "Registro duplicado.";

                _logger.LogWarning(ex, "Violação de unicidade capturada. Constraint={Constraint}", pex.ConstraintName);

                await WriteProblem(ctx, StatusCodes.Status409Conflict, "Conflito de dados", friendly);
                return;
            }

            _logger.LogError(ex, "Erro de banco não tratado. SqlState={SqlState}", pex.SqlState);
            await WriteProblem(ctx, StatusCodes.Status400BadRequest, "Erro de dados", "Erro ao persistir dados.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            if (ctx.Response.HasStarted)
            {
                _logger.LogError(ex, "ArgumentOutOfRange após resposta iniciada. TraceId={TraceId}", ctx.TraceIdentifier);
                return;
            }

            _logger.LogWarning(ex, "Valor inválido para parâmetro {ParamName}", ex.ParamName);

            var param = ex.ParamName ?? "parâmetro";
            var value = ex.ActualValue?.ToString() ?? "desconhecido";
            var detail = $"O valor enviado para '{param}' não é aceito. Valor recebido: '{value}'.";

            await WriteProblem(ctx, StatusCodes.Status400BadRequest, "Parâmetro inválido", detail);
        }
        catch (Exception ex)
        {
            if (ctx.Response.HasStarted)
            {
                _logger.LogError(ex, "Erro inesperado após resposta iniciada. TraceId={TraceId}", ctx.TraceIdentifier);
                return;
            }
            var traceId = ctx.TraceIdentifier;
            _logger.LogError(ex, "Erro inesperado. TraceId: {TraceId}", traceId);
            await WriteProblem(ctx, StatusCodes.Status500InternalServerError, "Erro interno no servidor", $"Ocorreu um erro inesperado. TraceId: {traceId}");
        }
    }

    private static async Task WriteProblem(HttpContext ctx, int status, string title, string detail)
    {
        var pd = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = ctx.Request.Path
        };

        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(pd, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await ctx.Response.WriteAsync(json);
    }
}