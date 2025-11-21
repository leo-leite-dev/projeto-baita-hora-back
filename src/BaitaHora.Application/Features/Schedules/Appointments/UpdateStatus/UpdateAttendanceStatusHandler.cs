using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Schedules.Appointments.UpdateStatus;

public sealed class UpdateAttendanceStatusHandler
    : IRequestHandler<UpdateAttendanceStatusCommand, Result<Unit>>
{
    private readonly UpdateAttendanceStatusUseCase _useCase;

    public UpdateAttendanceStatusHandler(UpdateAttendanceStatusUseCase useCase)
        => _useCase = useCase;

    public Task<Result<Unit>> Handle(
        UpdateAttendanceStatusCommand request,
        CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}