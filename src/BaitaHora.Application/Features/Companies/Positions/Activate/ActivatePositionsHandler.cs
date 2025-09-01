using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Activate;

public sealed class ActivatePositionsHandler
    : IRequestHandler<ActivatePositionsCommand, Result<ActivatePositionsResponse>>
{
    private readonly ActivatePositionsUseCase _useCase;

    public ActivatePositionsHandler(ActivatePositionsUseCase useCase)
        => _useCase = useCase;

    public Task<Result<ActivatePositionsResponse>> Handle(
        ActivatePositionsCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}