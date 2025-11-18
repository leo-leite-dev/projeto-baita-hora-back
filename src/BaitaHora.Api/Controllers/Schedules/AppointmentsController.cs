using MediatR;
using BaitaHora.Api.Helpers;
using BaitaHora.Contracts.DTOs.Schedulings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Contracts.DTOs.Schedulings.Appointments;
using BaitaHora.Application.Features.Schedulings.Appointments.List;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Api.Mappers.Schedules;

namespace BaitaHora.Api.Controllers.Schedulings;

[ApiController]
[Route(ApiRoutes.SchedulesPrefix + "/appointments")]
[Authorize]
public sealed class AppointmentsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentCompany _currentCompany;

    public AppointmentsController(IMediator mediator, ICurrentCompany currentCompany)
    {
        _mediator = mediator;
        _currentCompany = currentCompany;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment(
        [FromBody] CreateAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(_currentCompany.Id);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPut("{appointmentId:guid}/reschedule")]
    public async Task<IActionResult> RescheduleAppointment(
    [FromRoute] Guid appointmentId,
    [FromBody] RescheduleAppointmentRequest req,
    CancellationToken ct)
    {
        var cmd = req.ToCommand(appointmentId, _currentCompany.Id);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPut("{appointmentId:guid}/cancel")]
    public async Task<IActionResult> CancelAppointment(
        [FromRoute] Guid appointmentId,
        [FromBody] CancelAppointmentRequest req,
        CancellationToken ct)
    {
        var cmd = req.ToCommand(appointmentId, _currentCompany.Id);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpPut("{appointmentId:guid}/no-show")]
    public async Task<IActionResult> NoShowAppointment(
    [FromRoute] Guid appointmentId,
    [FromBody] NoShowAppointmentRequest req,
    CancellationToken ct)
    {
        var cmd = req.ToCommand(appointmentId, _currentCompany.Id);
        var result = await _mediator.Send(cmd, ct);
        return result.ToActionResult(this);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyAppointments(
        [FromQuery] DateTime? dateUtc,
         CancellationToken ct)
    {
        var result = await _mediator.Send(new ListAppointmentsQuery(dateUtc), ct);
        return result.ToActionResult(this, result.Value);
    }
}