using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Owner;

public sealed class PatchOwnerHandler
    : IRequestHandler<PatchOwnerCommand, Result<PatchOwnerResponse>>
{
    private readonly PatchOwnerUseCase _useCase;

    public PatchOwnerHandler(PatchOwnerUseCase useCase)
        => _useCase = useCase;

    public Task<Result<PatchOwnerResponse>> Handle(
        PatchOwnerCommand request, CancellationToken ct)
        => _useCase.HandleAsync(request, ct);
}