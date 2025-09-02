using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Schedulings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Contracts.DTOs.Customers;
using BaitaHora.Api.Mappers.Customers;
using MediatR;

namespace BaitaHora.Api.Controllers.Schedulings;

[ApiController]
[Route(ApiRoutes.CustomersPrefix + "/{companyId:guid}/customers")]
[Authorize]
public sealed class CustomersController : ControllerBase
{
    private readonly ISender _mediator;
    public CustomersController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(
        [FromRoute] Guid companyId,
        [FromBody] CreateCustomerRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);

        return result.ToActionResult(this, result.Value);
    }
}