using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Remove;

public sealed class RemovePositionHandler
    : IRequestHandler<RemovePositionCommand, Result<RemovePositionResponse>>
{
    private readonly RemovePositionUseCase _useCase;
    public RemovePositionHandler(RemovePositionUseCase useCase)
        => _useCase = useCase;

    public Task<Result<RemovePositionResponse>> Handle(
        RemovePositionCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}
