using BaitaHora.Application.Features.Schedulings.Appointments.ReadModels;
using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.IRepositories.Schedulings;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByCompanyAndDateAsync(Guid companyId, DateTime dateUtc, CancellationToken ct);
    Task<IReadOnlyList<AppointmentDtoBase>> ListByCompanyAndDateAsync(Guid companyId, DateTime dateUtc, CancellationToken ct = default);
    Task<IReadOnlyList<AppointmentDto>> GetByScheduleIdAsync(Guid scheduleId, CancellationToken ct = default);
    Task<IReadOnlyList<Appointment>> GetPendingWithEndBeforeAsync(DateTime utcNow, CancellationToken ct);
}