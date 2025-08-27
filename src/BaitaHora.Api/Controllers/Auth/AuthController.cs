using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Contracts.DTOS.Auth;
using BaitaHora.Api.Mappers.Auth;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Api.Web.Cookies;
using BaitaHora.Application.Abstractions.Integrations;

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
    public IActionResult Me()
    {
        var user = HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
            return Unauthorized(new { message = "Usuário não autenticado" });

        var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                     ?? user.FindFirst("sub")?.Value;
        var username = user.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                       ?? user.Identity?.Name;
        var roles = user.FindAll(System.Security.Claims.ClaimTypes.Role).Select(r => r.Value).ToList();

        return Ok(new
        {
            IsAuthenticated = true,
            UserId = userId,
            Username = username,
            Roles = roles
        });
    }

    [HttpGet("diagnose")]
    public async Task<IActionResult> Diagnose()
    {
        await _ig.DiagnoseAsync(
            "<TEU_PAGE_TOKEN>",
            "<TEU_APP_ID>",
            "<TEU_APP_SECRET>",
            "<TEU_IG_USER_ID>",
            HttpContext.RequestAborted);

        return Ok("Diagnose executado. Veja logs.");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthenticateRequest req, CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);

        if (!result.IsSuccess)
            return result.ToActionResult(this, result.Error);

        var accessToken = result.Value.AccessToken;

        var now = DateTime.UtcNow;
        var ttl = result.Value.ExpiresAtUtc > now
            ? result.Value.ExpiresAtUtc - now
            : TimeSpan.FromDays(7);

        var cookie = _cookieFactory.CreateLoginCookie(accessToken, ttl);
        _cookieWriter.Write(Response, cookie);

        return Ok(result.Value);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        var cookie = _cookieFactory.CreateLogoutCookie();
        _cookieWriter.Write(Response, cookie);
        return Ok(new { message = "Logout efetuado." });
    }

    [HttpPost("register-owner")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterOwner([FromBody] RegisterOwnerWithCompanyRequest req, CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }
}