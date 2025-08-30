using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Catalog.Create;

public sealed class CreateServiceOfferingHandler
    : IRequestHandler<CreateServiceOfferingCommand, Result<CreateServiceOfferingResponse>>
{
    private readonly CreateServiceOfferingUseCase _useCase;
    public CreateServiceOfferingHandler(CreateServiceOfferingUseCase useCase)
    => _useCase = useCase;

    public Task<Result<CreateServiceOfferingResponse>> Handle(
        CreateServiceOfferingCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}