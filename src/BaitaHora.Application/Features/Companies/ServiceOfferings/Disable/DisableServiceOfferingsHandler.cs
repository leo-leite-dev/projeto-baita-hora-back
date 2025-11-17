using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Disable;

public sealed class DisableServiceOfferingsHandler
    : IRequestHandler<DisableServiceOfferingsCommand, Result<Unit>>
{
    private readonly DisableServiceOfferingsUseCase _useCase;

    public DisableServiceOfferingsHandler(DisableServiceOfferingsUseCase useCase)
        => _useCase = useCase;

    public Task<Result<Unit>> Handle(
        DisableServiceOfferingsCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}