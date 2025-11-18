using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.Create;

public sealed class CreateAppointmentHandler
    : IRequestHandler<CreateAppointmentCommand, Result<Guid>>
{
    private readonly CreateAppointmentUseCase _useCase;
    public CreateAppointmentHandler(CreateAppointmentUseCase useCase) => _useCase = useCase;

    public Task<Result<Guid>> Handle(CreateAppointmentCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}