using System.Security.Claims;
using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Auth;
using BaitaHora.Api.Web.Cookies;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Abstractions.Integrations;
using BaitaHora.Contracts.DTOS.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<AuthenticateResponse> Me([FromServices] ICurrentUser current)
    {
        if (!current.IsAuthenticated)
            return Unauthorized(new { message = "Usuário não autenticado." });

        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value);
        var companyId = User.FindFirst("companyId")?.Value;
        var memberId = User.FindFirst("memberId")?.Value;

        return Ok(new AuthenticateResponse(
            IsAuthenticated: true,
            UserId: current.UserId.ToString(),
            Username: current.Username.Value,
            Roles: roles,
            CompanyId: companyId,
            MemberId: memberId
        ));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthenticateRequest req, CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
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