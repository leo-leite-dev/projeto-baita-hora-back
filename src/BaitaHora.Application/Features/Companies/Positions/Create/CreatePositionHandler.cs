using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed class CreatePositionHandler
    : IRequestHandler<CreatePositionCommand, Result>
{
    private readonly CreatePositionUseCase _useCase;

    public CreatePositionHandler(CreatePositionUseCase useCase)
        => _useCase = useCase;

    public Task<Result>  Handle(
        CreatePositionCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}