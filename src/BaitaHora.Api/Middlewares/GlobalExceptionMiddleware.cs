// using System.Diagnostics;
// using System.Net;
// using System.Text.Json;
// using Microsoft.AspNetCore.Mvc;

// namespace BaitaHora.Api.Middlewares;

// public sealed class GlobalExceptionMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly ILogger<GlobalExceptionMiddleware> _log;
//     private readonly IWebHostEnvironment _env;

//     public GlobalExceptionMiddleware(
//         RequestDelegate next,
//         ILogger<GlobalExceptionMiddleware> log,
//         IWebHostEnvironment env)
//     {
//         _next = next;
//         _log = log;
//         _env = env;
//     }

//     public async Task Invoke(HttpContext ctx)
//     {
//         try
//         {
//             await _next(ctx);
//         }
//         catch (Exception ex)
//         {
//             // se já começou a resposta, não dá pra trocar o body
//             if (ctx.Response.HasStarted)
//             {
//                 _log.LogWarning(ex, "Response already started. Cannot write problem details. Path={Path}", ctx.Request.Path);
//                 throw; // deixa o servidor encerrar
//             }

//             await HandleAsync(ctx, ex);
//         }
//     }

//     private async Task HandleAsync(HttpContext ctx, Exception ex)
//     {
//         // correlation / trace id (distribuído)
//         var correlationId = Activity.Current?.Id ?? ctx.TraceIdentifier;
//         ctx.Response.Headers["X-Correlation-Id"] = correlationId;

//         // Mapear status e título por tipo de exceção
//         var (status, title) = ex switch
//         {
//             // Domínio / negócio
//             BaitaHora.Domain.Commons.Exceptions.UserException         => (HttpStatusCode.BadRequest,  "Erro de negócio"),
//             BaitaHora.Domain.Commons.Exceptions.ForbiddenException    => (HttpStatusCode.Forbidden,   "Acesso negado"),
//             BaitaHora.Domain.Commons.Exceptions.NotFoundException     => (HttpStatusCode.NotFound,    "Recurso não encontrado"),

//             // Validação (FluentValidation, por ex.)
//             FluentValidation.ValidationException                      => (HttpStatusCode.BadRequest,  "Validação falhou"),

//             // Autorização / autenticação
//             UnauthorizedAccessException                                => (HttpStatusCode.Forbidden,   "Acesso negado"),

//             // EF / dados
//             Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException => (HttpStatusCode.Conflict,    "Conflito de concorrência"),
//             Microsoft.EntityFrameworkCore.DbUpdateException            => (HttpStatusCode.Conflict,    "Erro ao persistir dados"),

//             // Cancelamentos / timeouts
//             OperationCanceledException                                  => ((HttpStatusCode)499,        "Requisição cancelada"),
//             TimeoutException                                             => (HttpStatusCode.GatewayTimeout, "Tempo esgotado"),

//             // Default: bug/infra
//             _ => (HttpStatusCode.InternalServerError, "Erro interno")
//         };

//         // Logging adequado por severidade
//         if (ex is OperationCanceledException)
//         {
//             _log.LogInformation("Request canceled by client. Status={Status} CorrelationId={CorrelationId} Path={Path}",
//                 (int)status, correlationId, ctx.Request.Path);
//         }
//         else if ((int)status >= 500)
//         {
//             _log.LogError(ex, "Unhandled exception. Status={Status} CorrelationId={CorrelationId} Path={Path}",
//                 (int)status, correlationId, ctx.Request.Path);
//         }
//         else
//         {
//             _log.LogWarning(ex, "Business/Expected exception. Status={Status} CorrelationId={CorrelationId} Path={Path}",
//                 (int)status, correlationId, ctx.Request.Path);
//         }

//         var problem = new ProblemDetails
//         {
//             Type     = $"https://httpstatuses.com/{(int)status}",
//             Title    = title,
//             Status   = (int)status,
//             Detail   = _env.IsDevelopment() ? ex.Message : null,
//             Instance = ctx.Request.Path
//         };
//         problem.Extensions["correlationId"] = correlationId;

//         ctx.Response.ContentType = "application/problem+json";
//         ctx.Response.StatusCode  = (int)status;

//         var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
//         {
//             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//         });

//         await ctx.Response.WriteAsync(json);
//     }
// }