using System.Security.Claims;
using BaitaHora.Application.Abstractions.Auth;

namespace BaitaHora.Api.Web.Middlewares;

public sealed class JwtCookieAuthenticationMiddleware
{
    public const string CookieName = "jwtToken";
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtCookieAuthenticationMiddleware> _log;

    public JwtCookieAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<JwtCookieAuthenticationMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        string? token = null;

        if (context.Request.Cookies.TryGetValue(CookieName, out var cookieToken) && !string.IsNullOrWhiteSpace(cookieToken))
        {
            token = cookieToken;
        }
        else
        {
            var auth = context.Request.Headers.Authorization.ToString();
            if (auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = auth.Substring("Bearer ".Length).Trim();
        }

        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                var principal = tokenService.ValidateToken(token);

                if (principal is not null && principal.Identity is ClaimsIdentity id && id.IsAuthenticated)
                {
                    context.User = principal;
                    _log.LogDebug("JWT válido. Usuário autenticado: {Name}", principal.Identity?.Name);
                }
                else
                {
                    _log.LogInformation("JWT inválido ou não autenticado.");
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Falha ao validar JWT.");
            }
        }
        else
        {
            _log.LogTrace("Nenhum JWT encontrado (cookie ou Authorization header).");
        }

        await _next(context);
    }
}