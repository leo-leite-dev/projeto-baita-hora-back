using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Companies;
using BaitaHora.Contracts.DTOs.Companies.Company.Create;
using BaitaHora.Contracts.DTOs.Companies.Company.Patch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/{companyId:guid}/ServiceOfferings")]
[Authorize]
public sealed class ServiceOfferingsController : ControllerBase
{
    private readonly ISender _mediator;
    public ServiceOfferingsController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateService(
        [FromRoute] Guid companyId,
        [FromBody] CreateServiceOfferingRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{serviceOfferingId:guid}")]
    public async Task<IActionResult> PatchService(
        [FromRoute] Guid companyId,
        [FromRoute] Guid serviceOfferingId,
        [FromBody] PatchServiceOfferingRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId, serviceOfferingId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{serviceOfferingId:guid}")]
    public async Task<IActionResult> RemoveService(
        [FromRoute] Guid companyId,
        [FromRoute] Guid serviceOfferingId,
        CancellationToken ct)
    {
        var cmd = ServiceOfferingsApiMappers.ToCommand(companyId, serviceOfferingId);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("disable")]
    public async Task<IActionResult> DisableService(
       [FromRoute] Guid companyId,
       [FromBody] DisableServiceOfferingsRequest req,
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
        [FromBody] ActivateServiceOfferingsRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }
}