using BaitaHora.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Application.Features.Companies.Stats.Get.List;
using MediatR;

namespace BaitaHora.Api.Controllers.Companies;

[ApiController]
[Route(ApiRoutes.SchedulesPrefix + "/stats")]
[Authorize]
public sealed class CompanyStatsController : ControllerBase
{
    private readonly ISender _mediator;

    public CompanyStatsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats(
        [FromQuery] DateTime? dateUtc,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ListCompanyStatsQuery(dateUtc), ct);
        return result.ToActionResult(this);
    }
}