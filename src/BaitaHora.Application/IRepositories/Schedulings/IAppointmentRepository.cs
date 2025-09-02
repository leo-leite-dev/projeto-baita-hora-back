using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.IRepositories.Schedulings;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByScheduleAsync(Guid scheduleId, CancellationToken ct = default);
}
