using MediatR;
using BaitaHora.Application.Common.Results;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Remove;

public sealed class RemoveServiceOfferingHandler
    : IRequestHandler<RemoveServiceOfferingCommand, Result<Unit>>
{
    private readonly RemoveServiceOfferingUseCase _useCase;
    public RemoveServiceOfferingHandler(RemoveServiceOfferingUseCase useCase) => _useCase = useCase;

    public Task<Result<Unit>> Handle(RemoveServiceOfferingCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}