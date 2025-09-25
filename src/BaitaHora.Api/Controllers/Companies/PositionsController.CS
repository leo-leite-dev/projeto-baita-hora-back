using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Application.Features.Companies.Positions.Remove;
using BaitaHora.Application.Features.Companies.Positions.Remove.ServicesFromPosition;
using BaitaHora.Contracts.DTOs.Companies.Positions;
using BaitaHora.Application.Features.Companies.ListPositions.Get.List;
using MediatR;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/positions")]
[Authorize]
public sealed class PositionsController : ControllerBase
{
    private readonly ISender _mediator;

    public PositionsController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreatePosition(
        [FromBody] CreatePositionRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{positionId:guid}")]
    public async Task<IActionResult> PatchPosition(
        [FromRoute] Guid positionId,
        [FromBody] PatchPositionRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(positionId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{positionId:guid}")]
    public async Task<IActionResult> RemovePosition(
        [FromRoute] Guid positionId,
        CancellationToken ct)
    {
        var cmd = new RemovePositionCommand
        {
            PositionId = positionId
        };

        var result = await _mediator.Send(cmd, ct);
        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpDelete("{positionId:guid}/services")]
    public async Task<IActionResult> RemoveServicesFromPosition(
        [FromRoute] Guid positionId,
        [FromBody] RemoveServicesFromPositionRequest req,
        CancellationToken ct)
    {
        var cmd = new RemoveServicesFromPositionCommand
        {
            PositionId = positionId,
            ServiceOfferingIds = (req?.ServiceOfferingIds ?? Enumerable.Empty<Guid>()).ToArray()
        };

        var result = await _mediator.Send(cmd, ct);
        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("disable")]
    public async Task<IActionResult> DisablePosition(
        [FromBody] DisablePositionsRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);
        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("activate")]
    public async Task<IActionResult> ActivateMany(
        [FromBody] ActivatePositionsRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand();
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
}