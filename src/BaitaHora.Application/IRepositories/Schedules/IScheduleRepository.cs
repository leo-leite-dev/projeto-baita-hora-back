using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.Features.Schedules;

public interface IScheduleRepository : IGenericRepository<Schedule>
{
    Task<bool> ExistsForUserAsync(Guid userId, CancellationToken ct);
}