using MediatR;
using BaitaHora.Application.Common.Results;

namespace BaitaHora.Application.Features.Companies.Positions.Remove;

public sealed class RemovePositionHandler
    : IRequestHandler<RemovePositionCommand, Result<Unit>>
{
    private readonly RemovePositionUseCase _useCase;

    public RemovePositionHandler(RemovePositionUseCase useCase)
        => _useCase = useCase;

    public Task<Result<Unit>> Handle(RemovePositionCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}