using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.List;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ById;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.Combo;
using MediatR;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/service-offerings")]
[Authorize]
public sealed class ServiceOfferingsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<ServiceOfferingsController> _logger;

    public ServiceOfferingsController(ISender mediator, ILogger<ServiceOfferingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateService(
        [FromBody] CreateServiceOfferingRequest req,
        CancellationToken ct)
    {
        var cid = HttpContext.User.FindFirst("companyId")?.Value;
        _logger.LogInformation("CreateServiceOffering: cid={cid}", cid ?? "(null)");

        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{serviceOfferingId:guid}")]
    public async Task<IActionResult> PatchService(
        [FromRoute] Guid serviceOfferingId,
        [FromBody] PatchServiceOfferingRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(serviceOfferingId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{serviceOfferingId:guid}")]
    public async Task<IActionResult> RemoveService(
        [FromRoute] Guid serviceOfferingId,
        CancellationToken ct)
    {
        var cmd = ServiceOfferingsApiMappers.ToCommand(serviceOfferingId);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("disable")]
    public async Task<IActionResult> DisableService(
        [FromBody] DisableServiceOfferingsRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);

        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("activate")]
    public async Task<IActionResult> ActivateMany(
        [FromBody] ActivateServiceOfferingsRequest req,
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
        var query = new ListServiceOfferingsQuery();
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{serviceOfferingId:guid}")]
    public async Task<IActionResult> GetById(Guid serviceOfferingId, CancellationToken ct)
    {
        var query = new GetServiceOfferingByIdQuery(serviceOfferingId);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("combo")]
    public async Task<IActionResult> ListActiveCombo(
        [FromQuery] string? search,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        var query = new ListActiveServiceOfferingsComboQuery(search, take);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }
}