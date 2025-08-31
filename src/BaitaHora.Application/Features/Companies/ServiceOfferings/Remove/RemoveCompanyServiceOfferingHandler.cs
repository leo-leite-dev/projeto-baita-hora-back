using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Remove;

public sealed class RemoveServiceOfferingHandler
    : IRequestHandler<RemoveServiceOfferingCommand, Result<RemoveServiceOfferingResponse>>
{
    private readonly RemoveServiceOfferingUseCase _useCase;
    public RemoveServiceOfferingHandler(RemoveServiceOfferingUseCase useCase)
        => _useCase = useCase;

    public Task<Result<RemoveServiceOfferingResponse>> Handle(
        RemoveServiceOfferingCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}
