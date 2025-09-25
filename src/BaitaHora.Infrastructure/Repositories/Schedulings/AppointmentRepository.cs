using BaitaHora.Application.IRepositories.Schedulings;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Data.Repositories.Schedulings;

public sealed class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(AppDbContext context) : base(context) { }


    public Task<IReadOnlyList<Appointment>> GetByCompanyAndDateAsync(Guid companyId, DateTime date, CancellationToken ct = default)
        => _set.AsNoTracking()
               .Where(a => a.CompanyId == companyId
                        && a.StartsAtUtc.Date == date.Date
                        && a.IsActive)
               .OrderBy(a => a.StartsAtUtc)
               .ToListAsync(ct)
               .ContinueWith<IReadOnlyList<Appointment>>(t => t.Result, ct);
}