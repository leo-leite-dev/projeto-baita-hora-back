using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Companies;
using BaitaHora.Contracts.DTOs.Companies.Company.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Application.Features.Companies.Positions.Remove;
using BaitaHora.Contracts.DTOs.Companies.Company.Remove;
using BaitaHora.Application.Features.Companies.Positions.Remove.ServicesFromPosition;
using MediatR;
using BaitaHora.Contracts.DTOs.Companies.Positions.Disable;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/{companyId:guid}/positions")]
[Authorize]
public sealed class PositionsController : ControllerBase
{
    private readonly ISender _mediator;
    public PositionsController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreatePosition(
        [FromRoute] Guid companyId,
        [FromBody] CreatePositionRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPatch("{positionId:guid}")]
    public async Task<IActionResult> PatchPosition(
        [FromRoute] Guid companyId,
        [FromRoute] Guid positionId,
        [FromBody] PatchPositionRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId, positionId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpDelete("{positionId:guid}")]
    public async Task<IActionResult> RemovePosition(
        [FromRoute] Guid companyId,
        [FromRoute] Guid positionId,
        CancellationToken ct)
    {
        var cmd = new RemovePositionCommand
        {
            CompanyId = companyId,
            PositionId = positionId
        };

        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpDelete("{positionId:guid}/services")]
    public async Task<IActionResult> RemoveServicesFromPosition(
    [FromRoute] Guid companyId,
    [FromRoute] Guid positionId,
    [FromBody] RemoveServicesFromPositionRequest req,
    CancellationToken ct)
    {
        var cmd = new RemoveServicesFromPositionCommand
        {
            CompanyId = companyId,
            PositionId = positionId,
            ServiceOfferingIds = (req?.ServiceOfferingIds ?? Enumerable.Empty<Guid>()).ToArray()
        };

        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("disable")]
    public async Task<IActionResult> DisablePosition(
       [FromRoute] Guid companyId,
       [FromBody] DisablePositionsRequest req,
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
        [FromBody] ActivatePositionsRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }
}