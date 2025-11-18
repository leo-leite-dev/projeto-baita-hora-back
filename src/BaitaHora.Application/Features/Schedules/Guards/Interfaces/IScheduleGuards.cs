using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.Features.Schedules.Guards.Interfaces;

public interface IScheduleGuards
{
    Task<Result<(Appointment Appointment, Schedule Schedule)>> EnsureAppointmentBelongsToMemberAsync(Guid appointmentId, Guid memberId, CancellationToken ct);
}
