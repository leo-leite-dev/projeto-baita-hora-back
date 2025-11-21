using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Contracts.DTOs.Companies.ServiceOfferings;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.List;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.Combo;
using MediatR;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.Options;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Features.Companies.ServiceOffering.Get.ById;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/service-offerings")]
[Authorize]
public sealed class ServiceOfferingsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentCompany _currentCompany;

    public ServiceOfferingsController(IMediator mediator, ICurrentCompany currentCompany)
    {
        _mediator = mediator;
        _currentCompany = currentCompany;
    }

    [HttpPost]
    public async Task<IActionResult> CreateService(
        [FromBody] CreateServiceOfferingRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(_currentCompany.Id); 
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{serviceOfferingId:guid}")]
    public async Task<IActionResult> PatchService(
        [FromRoute] Guid serviceOfferingId,
        [FromBody] PatchServiceOfferingRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(serviceOfferingId, _currentCompany.Id);  
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{serviceOfferingId:guid}")]
    public async Task<IActionResult> RemoveService(
        [FromRoute] Guid serviceOfferingId,
        CancellationToken ct)
    {
        var cmd = serviceOfferingId.ToCommand(_currentCompany.Id); 
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPatch("disable")]
    public async Task<IActionResult> DisableService(
        [FromBody] DisableServiceOfferingsRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(_currentCompany.Id);  
        var result = await _mediator.Send(cmd, ct);
        if (result.IsSuccess) return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("activate")]
    public async Task<IActionResult> ActivateMany(
        [FromBody] ActivateServiceOfferingsRequest req,
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
        var query = new ListServiceOfferingsQuery();
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("{serviceOfferingId:guid}")]
    public async Task<IActionResult> GetById(Guid serviceOfferingId, CancellationToken ct)
    {
        var query = new ListServiceOfferingByIdQuery(serviceOfferingId);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("options")]
    public async Task<IActionResult> ListActiveCombo(
        [FromQuery] string? search,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        var query = new ListActiveServiceOfferingsOptionsQuery(search, take);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("my-options")]
    public async Task<IActionResult> ListActiveOptionsForCurrentMember(
        [FromQuery] string? search,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        var query = new ListActiveServiceOfferingsForMemberOptionsQuery(search, take);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }
}