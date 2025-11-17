using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Schedulings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Contracts.DTOs.Customers;
using BaitaHora.Api.Mappers.Customers;
using MediatR;
using BaitaHora.Application.Features.Customers.Get.List;

namespace BaitaHora.Api.Controllers.Schedulings;

[ApiController]
[Route(ApiRoutes.CustomersPrefix)]
[Authorize]
public sealed class CustomersController : ControllerBase
{
    private readonly ISender _mediator;
    public CustomersController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateCustomerRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("options")]
    public async Task<IActionResult> GetCustomerOptions(
    [FromQuery] string? search,
    CancellationToken ct)
    {
        var query = new ListCustomersQuery(search);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this);
    }
}