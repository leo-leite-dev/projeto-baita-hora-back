using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.IRepositories.Schedulings;

public interface IScheduleRepository : IGenericRepository<Schedule>
{
    Task<bool> ExistsForUserAsync(Guid memberId, CancellationToken ct = default);
    Task<Schedule?> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default);
    Task<Schedule?> GetWithAppointmentsByMemberIdAsync(Guid memberId, CancellationToken ct = default);
}