using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed class PatchCompanyPositionHandler
    : IRequestHandler<PatchCompanyPositionCommand, Result<PatchCompanyPositionResponse>>
{
    private readonly PatchCompanyPositionUseCase _useCase;

    public PatchCompanyPositionHandler(PatchCompanyPositionUseCase useCase)
        => _useCase = useCase;

    public Task<Result<PatchCompanyPositionResponse>> Handle(
        PatchCompanyPositionCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}