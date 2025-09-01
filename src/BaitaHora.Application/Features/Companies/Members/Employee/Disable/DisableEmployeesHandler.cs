using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Employees.Disable;

public sealed class DisableEmployeesHandler
    : IRequestHandler<DisableEmployeesCommand, Result<DisableEmployeesResponse>>
{
    private readonly DisableEmployeesUseCase _useCase;
    
    public DisableEmployeesHandler(DisableEmployeesUseCase useCase) => _useCase = useCase;

    public Task<Result<DisableEmployeesResponse>> Handle(
        DisableEmployeesCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}