using BaitaHora.Application.Features.Schedules.Get.ReadModels;
using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.IRepositories.Schedules;

public interface IScheduleRepository : IGenericRepository<Schedule>
{
    Task<bool> ExistsForMemberAsync(Guid memberId, CancellationToken ct = default);
    Task<Schedule?> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default);
    Task<Schedule?> GetWithAppointmentsByMemberIdAsync(Guid memberId, CancellationToken ct = default);
    Task<ScheduleDetailsDto?> GetDetailsByMemberIdAsync(Guid memberId, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct);
}
