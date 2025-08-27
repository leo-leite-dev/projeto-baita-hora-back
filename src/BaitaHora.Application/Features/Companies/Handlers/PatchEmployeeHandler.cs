using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Application.Features.Companies.Responses;
using BaitaHora.Application.Features.Companies.UseCases;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Handlers;

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