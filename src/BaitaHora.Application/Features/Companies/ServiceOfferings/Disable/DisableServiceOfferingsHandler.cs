using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOfferings.Disable;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Disable;

public sealed class DisableServiceOfferingsHandler
    : IRequestHandler<DisableServiceOfferingsCommand, Result<DisableServiceOfferingsResponse>>
{
    private readonly DisableServiceOfferingsUseCase _useCase;

    public DisableServiceOfferingsHandler(DisableServiceOfferingsUseCase useCase)
        => _useCase = useCase;

    public Task<Result<DisableServiceOfferingsResponse>> Handle(
        DisableServiceOfferingsCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}