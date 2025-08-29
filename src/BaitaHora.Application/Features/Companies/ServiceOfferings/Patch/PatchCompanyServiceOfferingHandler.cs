using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.ServiceOffering.Patch;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Catalog.Patch;

public sealed class PatchCompanyServiceOfferingHandler
    : IRequestHandler<PatchCompanyServiceOfferingCommand, Result<PatchCompanyServiceOfferingResponse>>
{
    private readonly PatchCompanyServiceOfferingUseCase _useCase;
    public PatchCompanyServiceOfferingHandler(PatchCompanyServiceOfferingUseCase useCase)
    => _useCase = useCase;

    public Task<Result<PatchCompanyServiceOfferingResponse>> Handle(
        PatchCompanyServiceOfferingCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}