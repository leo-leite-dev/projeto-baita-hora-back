using MediatR;
using BaitaHora.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Application.Features.Schedules.Appointments.List;
using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Contracts.DTOs.Schedules.Appointments;
using BaitaHora.Api.Mappers.Schedules.Appointments;
using BaitaHora.Application.Features.Schedules.Appointments.ListByMember;

namespace BaitaHora.Api.Controllers.Schedules;

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

    [HttpPut("{appointmentId:guid}/attendance")]
    public async Task<IActionResult> UpdateAttendance(
        [FromRoute] Guid appointmentId,
        [FromBody] UpdateAttendanceStatusRequest req,
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

    [HttpGet("member/{memberId:guid}")]
    public async Task<IActionResult> GetMemberAppointments(
        [FromRoute] Guid memberId,
        [FromQuery] DateTime? dateUtc,
        CancellationToken ct)
    {
        var query = new ListByMemberQuery(memberId, dateUtc);
        var result = await _mediator.Send(query, ct);
        return result.ToActionResult(this, result.Value);
    }
}