using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Employees.Activate;

public sealed class ActivateEmployeesHandler
    : IRequestHandler<ActivateEmployeesCommand, Result<ActivateEmployeesResponse>>
{
    private readonly ActivateEmployeesUseCase _useCase;

    public ActivateEmployeesHandler(ActivateEmployeesUseCase useCase) => _useCase = useCase;

    public Task<Result<ActivateEmployeesResponse>> Handle(
        ActivateEmployeesCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}