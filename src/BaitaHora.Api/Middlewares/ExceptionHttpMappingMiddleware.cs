using Microsoft.AspNetCore.Mvc;
using BaitaHora.Domain.Features.Commons.Exceptions;

public sealed class ExceptionHttpMappingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHttpMappingMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (CompanyException ex)
        {
            var pd = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflito de negócio",
                Detail = ex.Message
            };
            ctx.Response.StatusCode = StatusCodes.Status409Conflict;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsJsonAsync(pd);
        }
    }
}