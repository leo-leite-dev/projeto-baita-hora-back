using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Companies;
using BaitaHora.Contracts.DTOs.Companies.Company.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/{companyId:guid}/positions")]
[Authorize]
public sealed class CompanyPositionsController : ControllerBase
{
    private readonly ISender _mediator;
    public CompanyPositionsController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreatePosition(
        [FromRoute] Guid companyId,
        [FromBody] CreateCompanyPositionRequest req,
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
        [FromBody] PatchCompanyPositionRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId, positionId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }
}