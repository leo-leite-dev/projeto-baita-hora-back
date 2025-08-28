using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Responses;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Employee.Patch;

public sealed class PatchEmployeeHandler
    : IRequestHandler<PatchEmployeeCommand, Result<PatchEmployeeResponse>>
{
    private readonly PatchEmployeeUseCase _useCase;

    public PatchEmployeeHandler(PatchEmployeeUseCase useCase)
        => _useCase = useCase;

    public Task<Result<PatchEmployeeResponse>> Handle(
        PatchEmployeeCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}