using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Api.Contracts.Companies.Mappers;
using BaitaHora.Api.Contracts.Auth.Requests;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/{companyId:guid}/employees")]
[Authorize]
public sealed class CompanyEmployeesController : ControllerBase
{
    private readonly ISender _mediator;
    public CompanyEmployeesController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> RegisterEmployee([FromRoute] Guid companyId, [FromBody] RegisterEmployeeRequest req, CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }
}