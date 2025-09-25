using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Auth;
using BaitaHora.Api.Mappers.Companies;
using BaitaHora.Api.Mappers.Onboarding;
using BaitaHora.Contracts.DTOs.Companies.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BaitaHora.Api.Controllers.Members;

[ApiController]
[Route(ApiRoutes.MembersPrefix + "/{companyId:guid}")]
[Authorize]
public sealed class MembersController : ControllerBase
{
    private readonly ISender _mediator;
    public MembersController(ISender mediator) => _mediator = mediator;

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

    [HttpPatch("disable")]
    public async Task<IActionResult> DisableMany(
       [FromRoute] Guid companyId,
       [FromBody] DisableEmployeesRequest req,
       CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("activate")]
    public async Task<IActionResult> ActivateMany(
        [FromRoute] Guid companyId,
        [FromBody] ActivateEmployeesRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("{employeeId:guid}/position")]
    public async Task<IActionResult> ChangePrimaryPosition(
        [FromRoute] Guid companyId,
        [FromRoute] Guid employeeId,
        [FromBody] ChangeMemberPositionRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId, employeeId);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return Ok(result.Value);
        return result.ToActionResult(this);
    }
}