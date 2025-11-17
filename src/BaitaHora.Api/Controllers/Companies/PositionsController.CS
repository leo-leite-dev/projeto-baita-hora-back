using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Contracts.DTOs.Companies.Positions;
using BaitaHora.Application.Features.Companies.ListPositions.Get.List;
using BaitaHora.Application.Features.Companies.Positions.Get.ById;
using BaitaHora.Application.Features.Companies.Positions.Get.Combo;
using BaitaHora.Application.Abstractions.Auth;
using MediatR;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/positions")]
[Authorize]
public sealed class PositionsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentCompany _currentCompany;

    public PositionsController(IMediator mediator, ICurrentCompany currentCompany)
    {
        _mediator = mediator;
        _currentCompany = currentCompany;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePosition(
        [FromBody] CreatePositionRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(_currentCompany.Id);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{positionId:guid}")]
    public async Task<IActionResult> PatchPosition(
        [FromRoute] Guid positionId,
        [FromBody] PatchPositionRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(positionId, _currentCompany.Id);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{positionId:guid}")]
    public async Task<IActionResult> RemovePosition(
        [FromRoute] Guid positionId,
        CancellationToken ct)
    {
        var cmd = positionId.ToCommand(_currentCompany.Id);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("disable")]
    public async Task<IActionResult> DisablePosition(
        [FromBody] DisablePositionsRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(_currentCompany.Id);
        var result = await _mediator.Send(cmd, ct);
        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("activate")]
    public async Task<IActionResult> ActivateMany(
        [FromBody] ActivatePositionsRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(_currentCompany.Id);
        var result = await _mediator.Send(cmd, ct);
        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpGet]
    public async Task<IActionResult> ListAll(CancellationToken ct)
    {
        var query = new ListPositionsQuery();
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{positionId:guid}")]
    public async Task<IActionResult> GetById(Guid positionId, CancellationToken ct)
    {
        var query = new GetPositionByIdQuery(positionId);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("options")]
    public async Task<IActionResult> ListActiveOptions(
        [FromQuery] string? search,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        var query = new ListActivePositionsOptionsQuery(search, take);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }
}