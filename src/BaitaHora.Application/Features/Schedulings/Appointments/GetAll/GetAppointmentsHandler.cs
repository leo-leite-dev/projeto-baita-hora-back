using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Appointments.GetAll;

public sealed class GetAppointmentsHandler
    : IRequestHandler<GetAppointmentsQuery, Result<IReadOnlyList<GetAppointmentsResult>>>
{
    private readonly GetAppointmentsUseCase _useCase;

    public GetAppointmentsHandler(GetAppointmentsUseCase useCase)
        => _useCase = useCase;

    public Task<Result<IReadOnlyList<GetAppointmentsResult>>> Handle(
        GetAppointmentsQuery query, CancellationToken ct)
        => _useCase.HandleAsync(query, ct);
}
