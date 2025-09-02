using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Schedulings;
using BaitaHora.Contracts.DTOs.Schedulings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BaitaHora.Api.Controllers.Schedulings;

[ApiController]
[Route(ApiRoutes.CompaniesPrefix + "/{companyId:guid}/appointments")]
[Authorize]
public sealed class AppointmentsController : ControllerBase
{
    private readonly ISender _mediator;
    public AppointmentsController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateAppointment(
        [FromRoute] Guid companyId,
        [FromBody] CreateAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPut("{appointmentId:guid}/reschedule")]
    public async Task<IActionResult> RescheduleAppointment(
        [FromRoute] Guid companyId,
        [FromRoute] Guid appointmentId,
        [FromBody] RescheduleAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId, appointmentId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPut("{appointmentId:guid}/complete")]
    public async Task<IActionResult> CompleteAppointment(
        [FromRoute] Guid companyId,
        [FromRoute] Guid appointmentId,
        [FromBody] CompleteAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId, appointmentId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPut("{appointmentId:guid}/cancel")]
    public async Task<IActionResult> CancelAppointment(
        [FromRoute] Guid companyId,
        [FromRoute] Guid appointmentId,
        [FromBody] CancelAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(companyId, appointmentId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }
}