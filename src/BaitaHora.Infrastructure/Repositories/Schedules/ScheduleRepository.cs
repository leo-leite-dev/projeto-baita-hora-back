using BaitaHora.Application.Features.Schedules.Appointments.ReadModels;
using BaitaHora.Application.Features.Schedules.Get.ReadModels;
using BaitaHora.Application.IRepositories.Schedules;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Schedules;

public sealed class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(AppDbContext context) : base(context) { }

    public Task<bool> ExistsForMemberAsync(Guid memberId, CancellationToken ct = default)
        => _set.AnyAsync(s => s.MemberId == memberId, ct);

    public Task<Schedule?> GetByMemberIdAsync(Guid memberId, CancellationToken ct = default)
        => _set.AsNoTracking()
               .FirstOrDefaultAsync(s => s.MemberId == memberId, ct);

    public Task<Schedule?> GetWithAppointmentsByMemberIdAsync(Guid memberId, CancellationToken ct = default)
        => _set.AsNoTracking()
               .Include(s => s.Appointments)
               .FirstOrDefaultAsync(s => s.MemberId == memberId, ct);

    public async Task<ScheduleDetailsDto?> GetDetailsByMemberIdAsync(
        Guid memberId, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct)
    {
        return await _set
            .AsNoTracking()
            .Where(s => s.MemberId == memberId)
            .Select(s => new ScheduleDetailsDto(
                s.Id,
                s.MemberId,
                s.Appointments
                    .Where(a =>
                        (!fromUtc.HasValue || a.StartsAtUtc >= fromUtc) &&
                        (!toUtc.HasValue || a.StartsAtUtc < toUtc))
                    .OrderBy(a => a.StartsAtUtc)
                    .Select(a => new AppointmentDto
                    {
                        Id = a.Id,
                        CustomerId = a.CustomerId,
                        StartsAtUtc = a.StartsAtUtc,
                        EndsAtUtc = a.StartsAtUtc + a.Duration,
                        DurationMinutes = (int)a.Duration.TotalMinutes,
                        Status = a.Status.ToString()
                    })
                    .ToList()
            ))
            .SingleOrDefaultAsync(ct);
    }
}