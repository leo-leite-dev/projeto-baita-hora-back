using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Activate;

public sealed class ActivateServiceOfferingHandler
    : IRequestHandler<ActivateServiceOfferingCommand, Result<ActivateServiceOfferingResponse>>
{
    private readonly ActivateServiceOfferingUseCase _useCase;

    public ActivateServiceOfferingHandler(ActivateServiceOfferingUseCase useCase)
        => _useCase = useCase;

    public Task<Result<ActivateServiceOfferingResponse>> Handle(
        ActivateServiceOfferingCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}
