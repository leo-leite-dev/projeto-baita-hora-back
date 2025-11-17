using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Disable;

public sealed class DisablePositionsHandler
    : IRequestHandler<DisablePositionsCommand, Result<Unit>>
{
    private readonly DisablePositionsUseCase _useCase;

    public DisablePositionsHandler(DisablePositionsUseCase useCase)
        => _useCase = useCase;

    public Task<Result<Unit>> Handle(DisablePositionsCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}
