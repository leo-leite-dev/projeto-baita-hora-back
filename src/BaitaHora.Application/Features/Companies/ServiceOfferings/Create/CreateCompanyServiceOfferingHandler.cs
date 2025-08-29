using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Catalog.Create;

public sealed class CreateCompanyServiceOfferingHandler
    : IRequestHandler<CreateCompanyServiceOfferingCommand, Result<CreateCompanyServiceOfferingResponse>>
{
    private readonly CreateCompanyServiceOfferingUseCase _useCase;
    public CreateCompanyServiceOfferingHandler(CreateCompanyServiceOfferingUseCase useCase)
    => _useCase = useCase;

    public Task<Result<CreateCompanyServiceOfferingResponse>> Handle(
        CreateCompanyServiceOfferingCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}