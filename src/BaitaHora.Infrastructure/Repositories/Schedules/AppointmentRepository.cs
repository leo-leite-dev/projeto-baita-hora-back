using BaitaHora.Application.Features.Schedules.Appointments.ReadModels;
using BaitaHora.Application.IRepositories.Schedules;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Domain.Features.Schedules.Enums;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Schedules;

public sealed class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context) { }


    public async Task<IReadOnlyList<AppointmentDto>> GetByScheduleIdAsync(
        Guid scheduleId, CancellationToken ct = default)
    {
        var query =
            from a in _set.AsNoTracking()
                .Include(x => x.ServiceOfferings)
            join c in _context.Set<Customer>().AsNoTracking() on a.CustomerId equals c.Id
            where a.ScheduleId == scheduleId && a.IsActive
            orderby a.StartsAtUtc
            select new { Appointment = a, Customer = c };

        var data = await query.ToListAsync(ct);

        var result = data
            .Select(x => new AppointmentDto
            {
                Id = x.Appointment.Id,
                Status = x.Appointment.Status.ToString(),

                CustomerId = x.Customer.Id,
                CustomerName = x.Customer.Name,
                CustomerPhone = x.Customer.Phone.ToString(),

                ServiceOfferingIds = x.Appointment.ServiceOfferings
                    .Select(s => s.Id)
                    .ToArray(),

                ServiceOfferingNames = x.Appointment.ServiceOfferings
                    .Select(s => s.Name)
                    .ToArray(),

                AttendanceStatus = x.Appointment.AttendanceStatus,

                StartsAtUtc = x.Appointment.StartsAtUtc,
                EndsAtUtc = x.Appointment.EndsAtUtc,
                DurationMinutes = (int)x.Appointment.Duration.TotalMinutes
            })
            .ToList();

        return result;
    }

    public async Task<IReadOnlyList<Appointment>> GetPendingWithEndBeforeAsync(
        DateTime utcNow, CancellationToken ct = default)
    {
        return await _set
            .Include(a => a.ServiceOfferings)
            .Where(a =>
                a.IsActive &&
                a.Status == AppointmentStatus.Pending &&
                a.StartsAtUtc + a.Duration <= utcNow)
            .OrderBy(a => a.StartsAtUtc)
            .ToListAsync(ct);
    }
}