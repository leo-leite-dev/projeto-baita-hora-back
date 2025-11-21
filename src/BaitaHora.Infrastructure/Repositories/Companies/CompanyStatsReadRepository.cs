using BaitaHora.Application.Features.Companies.Stats.Get.ReadModels;
using BaitaHora.Application.Features.Companies.Stats.ReadModels;
using BaitaHora.Application.Features.Schedules.Appointments.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Domain.Features.Schedules.Enums;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companies;

public sealed class CompanyStatsReadRepository : ICompanyStatsReadRepository
{
    private readonly AppDbContext _context;

    public CompanyStatsReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyStatsDto> GetStatsAsync(
        Guid companyId,
        DateTime dateUtc,
        CancellationToken ct = default)
    {
        var startDay = DateTime.SpecifyKind(dateUtc.Date, DateTimeKind.Utc);
        var endDay = startDay.AddDays(1);

        var startMonth = new DateTime(dateUtc.Year, dateUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endMonth = startMonth.AddMonths(1);

        var baseQuery =
            from a in _context.Set<Appointment>().AsNoTracking()
                .Include(x => x.ServiceOfferings)
            join s in _context.Set<Schedule>().AsNoTracking() on a.ScheduleId equals s.Id
            join m in _context.Set<CompanyMember>().AsNoTracking() on s.MemberId equals m.Id
            where m.CompanyId == companyId
                  && a.IsActive && s.IsActive && m.IsActive
            select new { Appointment = a, Schedule = s, Member = m };

        var dayCount = await baseQuery
            .Where(x => x.Appointment.StartsAtUtc >= startDay && x.Appointment.StartsAtUtc < endDay)
            .CountAsync(ct);

        var monthCount = await baseQuery
            .Where(x => x.Appointment.StartsAtUtc >= startMonth && x.Appointment.StartsAtUtc < endMonth)
            .CountAsync(ct);

        var dayRevenue = await baseQuery
            .Where(x =>
                x.Appointment.StartsAtUtc >= startDay &&
                x.Appointment.StartsAtUtc < endDay &&
                x.Appointment.AttendanceStatus == AttendanceStatus.Attended)
            .SelectMany(x => x.Appointment.ServiceOfferings)
            .SumAsync(s => s.Price.Amount, ct);

        var monthRevenue = await baseQuery
            .Where(x =>
                x.Appointment.StartsAtUtc >= startMonth &&
                x.Appointment.StartsAtUtc < endMonth &&
                x.Appointment.AttendanceStatus == AttendanceStatus.Attended)
            .SelectMany(x => x.Appointment.ServiceOfferings)
            .SumAsync(s => s.Price.Amount, ct);

        var appointmentsBase = baseQuery.Select(x => x.Appointment);

        var dayAppointmentsQuery = appointmentsBase
            .Where(a => a.StartsAtUtc >= startDay && a.StartsAtUtc < endDay);

        var monthAppointmentsQuery = appointmentsBase
            .Where(a => a.StartsAtUtc >= startMonth && a.StartsAtUtc < endMonth);

        var daySummary = await BuildStatusSummaryAsync(dayAppointmentsQuery, ct);
        var monthSummary = await BuildStatusSummaryAsync(monthAppointmentsQuery, ct);

        var appointmentsQuery =
            from a in _context.Set<Appointment>()
                .AsNoTracking()
                .Include(x => x.ServiceOfferings)
            join s in _context.Set<Schedule>().AsNoTracking() on a.ScheduleId equals s.Id
            join m in _context.Set<CompanyMember>().AsNoTracking() on s.MemberId equals m.Id
            join u in _context.Set<User>().AsNoTracking() on m.UserId equals u.Id
            join up in _context.Set<UserProfile>().AsNoTracking() on u.ProfileId equals up.Id
            join c in _context.Set<Customer>().AsNoTracking() on a.CustomerId equals c.Id
            join p in _context.Set<CompanyPosition>().AsNoTracking()
                on m.PrimaryPositionId equals p.Id into posJoin
            from p in posJoin.DefaultIfEmpty()
            where m.CompanyId == companyId
                  && a.IsActive && s.IsActive && m.IsActive
                  && a.StartsAtUtc >= startDay && a.StartsAtUtc < endDay
            select new
            {
                Appointment = a,
                Customer = c,
                MemberId = m.Id,
                MemberName = up.Name,
                PositionName = p != null ? p.Name : null
            };

        var raw = await appointmentsQuery
            .OrderBy(x => x.Appointment.StartsAtUtc)
            .ToListAsync(ct);

        raw = raw
            .Where(x => x.Appointment.Status != AppointmentStatus.Cancelled)
            .ToList();

        var memberAppointments = raw
            .GroupBy(x => new { x.MemberId, x.MemberName, x.PositionName })
            .Select(g => new MemberAppointmentsDto
            {
                MemberId = g.Key.MemberId,
                MemberName = g.Key.MemberName,
                PositionName = g.Key.PositionName,
                Appointments = g
                    .Select(x =>
                    {
                        var a = x.Appointment;
                        var c = x.Customer;

                        return new AppointmentDto
                        {
                            Id = a.Id,
                            Status = a.Status.ToString(),

                            CustomerId = c.Id,
                            CustomerName = c.Name,
                            CustomerPhone = c.Phone.ToString(),

                            ServiceOfferingIds = a.ServiceOfferings
                                .Select(s => s.Id)
                                .ToArray(),

                            ServiceOfferingNames = a.ServiceOfferings
                                .Select(s => s.Name)
                                .ToArray(),

                            AttendanceStatus = a.AttendanceStatus,

                            StartsAtUtc = a.StartsAtUtc,
                            EndsAtUtc = a.EndsAtUtc,
                            DurationMinutes = (int)a.Duration.TotalMinutes
                        };
                    })
                    .OrderBy(a => a.StartsAtUtc)
                    .ToList()
            })
            .OrderBy(m => m.MemberName)
            .ToList();

        return new CompanyStatsDto
        {
            DayAppointmentsCount = dayCount,
            MonthAppointmentsCount = monthCount,
            DayRevenue = dayRevenue,
            MonthRevenue = monthRevenue,
            MemberAppointments = memberAppointments,

            DaySummary = daySummary,

            MonthSummary = new MonthSummaryDto
            {
                Cancelled = monthSummary.Cancelled,
                Unknown = monthSummary.Unknown
            }
        };
    }

    private static async Task<AppointmentStatusSummaryDto> BuildStatusSummaryAsync(
      IQueryable<Appointment> query,
      CancellationToken ct)
    {
        var pending = await query
            .Where(a => a.Status == AppointmentStatus.Pending)
            .CountAsync(ct);

        var finished = await query
            .Where(a => a.Status == AppointmentStatus.Finished)
            .CountAsync(ct);

        var cancelled = await query
            .Where(a => a.Status == AppointmentStatus.Cancelled)
            .CountAsync(ct);

        var attended = await query
            .Where(a => a.AttendanceStatus == AttendanceStatus.Attended)
            .CountAsync(ct);

        var noShow = await query
            .Where(a => a.AttendanceStatus == AttendanceStatus.NoShow)
            .CountAsync(ct);

        var unknown = await query
            .Where(a =>
                a.Status == AppointmentStatus.Finished &&
                a.AttendanceStatus == AttendanceStatus.Unknown)
            .CountAsync(ct);

        return new AppointmentStatusSummaryDto
        {
            Pending = pending,
            Finished = finished,
            Cancelled = cancelled,
            Attended = attended,
            NoShow = noShow,
            Unknown = unknown
        };
    }
}