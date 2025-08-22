using BaitaHora.Application.IServices.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var tokenService = context.RequestServices.GetRequiredService<ITokenService>();

        var token = context.Request.Cookies["jwtToken"];

        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine("[JwtMiddleware] jwtToken encontrado no cookie");
            var principal = tokenService.ValidateToken(token);

            if (principal != null)
            {
                Console.WriteLine("[JwtMiddleware] Token válido. Usuário autenticado.");
                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"[JwtMiddleware] Claim: {claim.Type} = {claim.Value}");
                }

                context.User = principal;
            }
            else
            {
                Console.WriteLine("[JwtMiddleware] Token inválido.");
            }
        }
        else
        {
            Console.WriteLine("[JwtMiddleware] Nenhum jwtToken encontrado no cookie");
        }

        await _next(context);
    }
}