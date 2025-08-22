using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Api.Contracts.Companies.Requests;
using BaitaHora.Api.Contracts.Companies.Mappers;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/{companyId:guid}/positions")]
[Authorize]
public sealed class CompanyPositionsController : ControllerBase
{
    private readonly ISender _mediator;
    public CompanyPositionsController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromRoute] Guid companyId, [FromBody] CreateCompanyPositionRequest req, CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }
}