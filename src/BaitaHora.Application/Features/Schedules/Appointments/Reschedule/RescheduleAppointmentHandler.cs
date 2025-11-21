using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Schedules.Appointments.Reschedule;

public sealed class RescheduleAppointmentHandler
    : IRequestHandler<RescheduleAppointmentCommand, Result<RescheduleAppointmentResponse>>
{
    private readonly RescheduleAppointmentUseCase _useCase;

    public RescheduleAppointmentHandler(RescheduleAppointmentUseCase useCase)
        => _useCase = useCase;

    public Task<Result<RescheduleAppointmentResponse>> Handle(
        RescheduleAppointmentCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}
