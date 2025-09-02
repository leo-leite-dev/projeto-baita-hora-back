using BaitaHora.Application.IRepositories.Schedulings;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Data.Repositories.Schedulings;

public sealed class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(AppDbContext context) : base(context) { }

    public Task<bool> ExistsForUserAsync(Guid userId, CancellationToken ct = default)
        => _set.AnyAsync(s => s.MemberId == userId, ct);

    public Task<Schedule?> GetByMemberIdAsync(Guid userId, CancellationToken ct = default)
        => _set.AsNoTracking()
               .FirstOrDefaultAsync(s => s.MemberId == userId, ct);

    public Task<Schedule?> GetWithAppointmentsByMemberIdAsync(Guid userId, CancellationToken ct = default)
        => _set.AsNoTracking()
               .Include(s => s.Appointments)
               .FirstOrDefaultAsync(s => s.MemberId == userId, ct);
}
