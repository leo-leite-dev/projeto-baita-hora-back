using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.NoShow;

public sealed class NoShowAppointmentHandler
    : IRequestHandler<NoShowAppointmentCommand, Result<Unit>>
{
    private readonly NoShowAppointmentUseCase _useCase;

    public NoShowAppointmentHandler(NoShowAppointmentUseCase useCase)
        => _useCase = useCase;

    public Task<Result<Unit>> Handle(
        NoShowAppointmentCommand request,
        CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}