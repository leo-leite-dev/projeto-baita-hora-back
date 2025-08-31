using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Positions.Remove.ServicesFromPosition;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Remove;

public sealed class RemoveServicesFromPositionHandler
    : IRequestHandler<RemoveServicesFromPositionCommand, Result<RemoveServicesFromPositionResponse>>
{
    private readonly RemoveServicesFromPositionUseCase _useCase;
    public RemoveServicesFromPositionHandler(RemoveServicesFromPositionUseCase useCase)
        => _useCase = useCase;

    public Task<Result<RemoveServicesFromPositionResponse>> Handle(
        RemoveServicesFromPositionCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}
