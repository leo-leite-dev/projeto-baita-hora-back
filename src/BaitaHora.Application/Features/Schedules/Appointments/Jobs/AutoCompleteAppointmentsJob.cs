using BaitaHora.Application.Common.Persistence;
using BaitaHora.Application.IRepositories.Schedulings;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Jobs;

public sealed class AutoCompleteAppointmentsJob : IAutoCompleteAppointmentsJob
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ITransactionalUnitOfWork _uow;

    public AutoCompleteAppointmentsJob(
        IAppointmentRepository appointmentRepository,
        ITransactionalUnitOfWork uow)
    {
        _appointmentRepository = appointmentRepository;
        _uow = uow;
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        var nowUtc = DateTime.UtcNow;

        var appointments = await _appointmentRepository
            .GetPendingWithEndBeforeAsync(nowUtc, ct);

        if (appointments.Count == 0)
            return;

        foreach (var appointment in appointments)
        {
            var changed = appointment.Finish();
            if (!changed)
                continue;

            await _appointmentRepository.UpdateAsync(appointment, ct);
        }

        await _uow.SaveChangesAsync(ct);
    }
}