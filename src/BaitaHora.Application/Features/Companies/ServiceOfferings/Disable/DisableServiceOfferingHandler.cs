using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Disable;

public sealed class DisableServiceOfferingHandler
    : IRequestHandler<DisableServiceOfferingCommand, Result<DisableServiceOfferingResponse>>
{
    private readonly DisableServiceOfferingUseCase _useCase;

    public DisableServiceOfferingHandler(DisableServiceOfferingUseCase useCase)
        => _useCase = useCase;

    public Task<Result<DisableServiceOfferingResponse>> Handle(
        DisableServiceOfferingCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}