using MediatR;
using BaitaHora.Api.Helpers;
using BaitaHora.Api.Mappers.Schedulings;
using BaitaHora.Application.Features.Schedulings.Appointments.GetAll;
using BaitaHora.Contracts.DTOs.Schedulings;
using BaitaHora.Contracts.DTOS.Schedulings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaitaHora.Api.Controllers.Schedulings;

[ApiController]
[Route("api/appointments")]
[Authorize]
public sealed class AppointmentsController : ControllerBase
{
    private readonly ISender _mediator;

    public AppointmentsController(ISender mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateAppointment(
        [FromBody] CreateAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand();
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPut("{appointmentId:guid}/reschedule")]
    public async Task<IActionResult> RescheduleAppointment(
        [FromRoute] Guid appointmentId,
        [FromBody] RescheduleAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(appointmentId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPut("{appointmentId:guid}/complete")]
    public async Task<IActionResult> CompleteAppointment(
        [FromRoute] Guid appointmentId,
        [FromBody] CompleteAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(appointmentId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpPut("{appointmentId:guid}/cancel")]
    public async Task<IActionResult> CancelAppointment(
        [FromRoute] Guid appointmentId,
        [FromBody] CancelAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(appointmentId);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this, result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetAppointmentsResponse>>> GetAll(
        [FromQuery] DateTime? date,
        CancellationToken ct)
    {
        var query = new GetAppointmentsQuery(date);
        var result = await _mediator.Send(query, ct);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        if (result.Value is not { } items)
            return Problem(
                detail: "No appointments found even though result was success.",
                statusCode: StatusCodes.Status500InternalServerError);

        return Ok(items.ToResponse());
    }
}