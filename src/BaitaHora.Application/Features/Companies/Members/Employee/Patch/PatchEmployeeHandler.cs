using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Employee.Patch;

public sealed class PatchEmployeeHandler
    : IRequestHandler<PatchEmployeeCommand, Result<Unit>>
{
    private readonly PatchEmployeeUseCase _useCase;

    public PatchEmployeeHandler(PatchEmployeeUseCase useCase)
        => _useCase = useCase;

    public Task<Result<Unit>> Handle(
        PatchEmployeeCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}