using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Patch;

public sealed class PatchServiceOfferingHandler
    : IRequestHandler<PatchServiceOfferingCommand, Result<PatchServiceOfferingResponse>>
{
    private readonly PatchServiceOfferingUseCase _useCase;
    public PatchServiceOfferingHandler(PatchServiceOfferingUseCase useCase)
    => _useCase = useCase;

    public Task<Result<PatchServiceOfferingResponse>> Handle(
        PatchServiceOfferingCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}