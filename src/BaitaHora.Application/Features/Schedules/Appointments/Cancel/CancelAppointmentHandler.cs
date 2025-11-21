using MediatR;
using BaitaHora.Application.Common.Results;

namespace BaitaHora.Application.Features.Schedules.Appointments.Cancel;

public sealed class CancelAppointmentHandler
    : IRequestHandler<CancelAppointmentCommand, Result<Unit>>
{
    private readonly CancelAppointmentUseCase _useCase;

    public CancelAppointmentHandler(CancelAppointmentUseCase useCase)
        => _useCase = useCase;

    public Task<Result<Unit>> Handle(CancelAppointmentCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}