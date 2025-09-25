using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Contracts.DTOS.Auth;
using BaitaHora.Api.Mappers.Auth;
using BaitaHora.Api.Web.Cookies;
using BaitaHora.Application.Abstractions.Integrations;
using BaitaHora.Application.Abstractions.Auth;

namespace BaitaHora.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IJwtCookieFactory _cookieFactory;
    private readonly IJwtCookieWriter _cookieWriter;
    private readonly IInstagramApi _ig;

    public AuthController(
        ISender mediator,
        IJwtCookieFactory cookieFactory,
        IInstagramApi ig,
        IJwtCookieWriter cookieWriter)
    {
        _mediator = mediator;
        _cookieFactory = cookieFactory;
        _ig = ig;
        _cookieWriter = cookieWriter;
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<AuthenticateResponse> Me()
    {
        var user = HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
            return Unauthorized(new { message = "Usuário não autenticado" });

        var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? user.FindFirst("sub")?.Value;
        var username = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                       ?? user.Identity?.Name;
        var roles = user.FindAll(System.Security.Claims.ClaimTypes.Role)
                        .Select(r => r.Value);

        return Ok(new AuthenticateResponse(
            IsAuthenticated: true,
            UserId: userId,
            Username: username,
            Roles: roles
        ));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthenticateRequest req, CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);

        if (!result.IsSuccess)
            return result.ToActionResult(this, result.Error);

        if (result.Value is not { } auth)
            return Problem(
                detail: "Auth result is null on success.",
                statusCode: StatusCodes.Status500InternalServerError);

        var now = DateTime.UtcNow;

        var ttl = auth.ExpiresAtUtc > now
            ? auth.ExpiresAtUtc - now
            : TimeSpan.FromDays(7);

        var cookie = _cookieFactory.CreateLoginCookie(auth.AccessToken, ttl);
        _cookieWriter.Write(Response, cookie);

        return Ok(auth.ToResponse());
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        var cookie = _cookieFactory.CreateLogoutCookie();
        _cookieWriter.Write(Response, cookie);
        return Ok(new { message = "Logout efetuado." });
    }
}