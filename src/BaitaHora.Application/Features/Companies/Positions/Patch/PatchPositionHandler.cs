using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed class PatchPositionHandler
    : IRequestHandler<PatchPositionCommand, Result>
{
    private readonly PatchPositionUseCase _useCase;

    public PatchPositionHandler(PatchPositionUseCase useCase)
        => _useCase = useCase;

    public Task<Result> Handle(
        PatchPositionCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}