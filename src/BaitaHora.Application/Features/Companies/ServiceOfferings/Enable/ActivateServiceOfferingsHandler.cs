using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOfferings.Enable;

public sealed class ActivateServiceOfferingsHandler
    : IRequestHandler<ActivateServiceOfferingsCommand, Result<ActivateServiceOfferingsResponse>>
{
    private readonly ActivateServiceOfferingsUseCase _useCase;

    public ActivateServiceOfferingsHandler(ActivateServiceOfferingsUseCase useCase)
        => _useCase = useCase;

    public Task<Result<ActivateServiceOfferingsResponse>> Handle(
        ActivateServiceOfferingsCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}