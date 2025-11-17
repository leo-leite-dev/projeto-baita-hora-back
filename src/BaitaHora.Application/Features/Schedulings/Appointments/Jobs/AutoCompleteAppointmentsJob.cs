using BaitaHora.Application.Common.Persistence;
using BaitaHora.Application.IRepositories.Schedulings;
using Microsoft.Extensions.Logging;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Jobs;

public sealed class AutoCompleteAppointmentsJob : IAutoCompleteAppointmentsJob
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ITransactionalUnitOfWork _uow;
    private readonly ILogger<AutoCompleteAppointmentsJob> _logger;

    public AutoCompleteAppointmentsJob(
        IAppointmentRepository appointmentRepository,
        ITransactionalUnitOfWork uow,
        ILogger<AutoCompleteAppointmentsJob> logger)
    {
        _appointmentRepository = appointmentRepository;
        _uow = uow;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        var nowUtc = DateTime.UtcNow;

        var appointments = await _appointmentRepository
            .GetPendingWithEndBeforeAsync(nowUtc, ct);

        if (appointments.Count == 0)
        {
            _logger.LogInformation(
                "AutoCompleteAppointmentsJob: nenhum agendamento pendente encontrado até {Now}.",
                nowUtc);
            return;
        }

        _logger.LogInformation(
            "AutoCompleteAppointmentsJob: auto-completando {Count} agendamentos até {Now}.",
            appointments.Count, nowUtc);

        foreach (var appointment in appointments)
        {
            var changed = appointment.MarkCompleted();
            if (!changed)
                continue;

            await _appointmentRepository.UpdateAsync(appointment, ct);
        }

        // 👇 AQUI é o que faltava
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("AutoCompleteAppointmentsJob: finalizado.");
    }
}
