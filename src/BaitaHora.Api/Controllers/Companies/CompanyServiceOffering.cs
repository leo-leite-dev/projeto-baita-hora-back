using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Companies;
using BaitaHora.Contracts.DTOs.Companies.Company.Create;
using BaitaHora.Contracts.DTOs.Companies.Company.Patch;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/{companyId:guid}/ServiceOfferings")]
[Authorize]
public sealed class CompanyServiceOfferingsController : ControllerBase
{
    private readonly ISender _mediator;
    public CompanyServiceOfferingsController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromRoute] Guid companyId,
        [FromBody] CreateCompanyServiceOfferingRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPatch("{serviceOfferingId:guid}")]
    public async Task<IActionResult> Patch(
        [FromRoute] Guid companyId,
        [FromRoute] Guid serviceOfferingId,
        [FromBody] PatchCompanyServiceOfferingRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId, serviceOfferingId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }
}