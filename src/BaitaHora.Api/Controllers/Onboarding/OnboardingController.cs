using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Contracts.DTOS.Auth;
using BaitaHora.Api.Mappers.Auth;
using BaitaHora.Api.Mappers.Onboarding;

namespace BaitaHora.Api.Controllers.Auth;

[ApiController]
[Route("api/onboarding")]
[Tags("Onboarding")]
public sealed class OnboardingController : ControllerBase
{
    private readonly ISender _mediator;

    public OnboardingController(ISender mediator)
    {
        _mediator = mediator;
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