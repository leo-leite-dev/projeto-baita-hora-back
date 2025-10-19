using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Companies;
using BaitaHora.Contracts.DTOs.Companies.Members;
using BaitaHora.Application.Features.Companies.ListMembers.Get.List;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Mappers.Onboarding;
using BaitaHora.Application.Features.Companies.Members.Get.ByUserId;
using MediatR;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/members")]
[Authorize]
public sealed class MembersController : ControllerBase
{
    private readonly ISender _mediator;

    public MembersController(ISender mediator) => _mediator = mediator;

    [HttpPost("employees")]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] RegisterEmployeeRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPatch("{memberId:guid}")]
    public async Task<IActionResult> PatchMember(
        [FromRoute] Guid memberId,
        [FromBody] PatchMemberRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(memberId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    // [HttpPatch("owner")]
    // public async Task<IActionResult> PatchOwner(
    //     [FromBody] PatchOwnerRequest req,
    //     CancellationToken ct)
    // {
    //     var cmd = req.ToCommand();
    //     var result = await _mediator.Send(cmd, ct);
    //     return result.ToActionResult(this, result.Value);
    // }

    [HttpPatch("disable")]
    public async Task<IActionResult> DisableMany(
        [FromBody] DisableEmployeesRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess)
            return NoContent();

        return result.ToActionResult(this);
    }

    [HttpPatch("activate")]
    public async Task<IActionResult> ActivateMany(
        [FromBody] ActivateEmployeesRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);
        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("{memberId:guid}/position")]
    public async Task<IActionResult> ChangePrimaryPosition(
        [FromRoute] Guid memberId,
        [FromBody] ChangeMemberPositionRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(memberId);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess)
            return Ok(result.Value);

        return result.ToActionResult(this);
    }

    [HttpGet]
    public async Task<IActionResult> ListAll(CancellationToken ct)
    {
        var query = new ListMembersQuery();
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{memberId:guid}/details")]
    public async Task<IActionResult> GetMemberFullDetails(Guid memberId, CancellationToken ct)
    {
        var query = new GetMemberProfileDetailsByMemberIdQuery(memberId);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{memberId:guid}")]
    public async Task<IActionResult> GetById(Guid memberId, CancellationToken ct)
    {
        var query = new GetMemberAdminEditByUserIdQuery(memberId);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }
}