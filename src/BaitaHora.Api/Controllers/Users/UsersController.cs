using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Auth;
using BaitaHora.Api.Mappers.Companies;
using BaitaHora.Api.Mappers.Onboarding;
using BaitaHora.Contracts.DTOs.Companies.Members;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaitaHora.Api.Controllers.Users;

[ApiController]
[Route(ApiRoutes.UsersPrefix + "/{companyId:guid}")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _mediator;
    public UsersController(ISender mediator) => _mediator = mediator;

    [HttpPost("employees")]
    public async Task<IActionResult> CreateEmployee(
        [FromRoute] Guid companyId,
        [FromBody] RegisterEmployeeRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPatch("employees/{employeeId:guid}")]
    public async Task<IActionResult> PatchEmployee(
        [FromRoute] Guid companyId,
        [FromRoute] Guid employeeId,
        [FromBody] PatchEmployeeRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId, employeeId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPatch("owner")]
    public async Task<IActionResult> PatchOwner(
        [FromRoute] Guid companyId,
        [FromBody] PatchOwnerRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }
}