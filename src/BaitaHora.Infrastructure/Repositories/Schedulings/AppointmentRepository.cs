using BaitaHora.Application.IRepositories.Schedulings;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Data.Repositories.Schedulings;

public sealed class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Appointment>> GetByScheduleAsync(Guid scheduleId, CancellationToken ct = default)
        => await _set.AsNoTracking()
                     .Where(a => a.ScheduleId == scheduleId)
                     .OrderBy(a => a.StartsAtUtc)
                     .ToListAsync(ct);
}