using BaitaHora.Application.Features.Schedules;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Data.Repositories.Schedulings;

public sealed class ScheduleRepository 
    : GenericRepository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(AppDbContext context) : base(context) { }

    public Task<bool> ExistsForUserAsync(Guid userId, CancellationToken ct)
        => _set.AnyAsync(a => a.UserId == userId, ct);
}