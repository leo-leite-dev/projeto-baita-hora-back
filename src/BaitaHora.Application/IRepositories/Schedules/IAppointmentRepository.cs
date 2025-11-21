using BaitaHora.Application.Features.Companies.Stats.Get.ReadModels;
using BaitaHora.Application.Features.Schedules.Appointments.ReadModels;
using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.IRepositories.Schedules;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IReadOnlyList<AppointmentDto>> GetByScheduleIdAsync(Guid scheduleId, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetPendingWithEndBeforeAsync(DateTime utcNow, CancellationToken ct);
}