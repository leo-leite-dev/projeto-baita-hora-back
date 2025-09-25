using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.IRepositories.Schedulings;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByCompanyAndDateAsync(Guid companyId, DateTime date, CancellationToken ct);
}
