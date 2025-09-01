using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Members.ChangePosition;
using MediatR;

namespace BaitaHora.Application.Companies.Features.Members.Promotion;

public sealed class ChangeMemberPositionHandler
    : IRequestHandler<ChangeMemberPositionCommand, Result<ChangeMemberPositionResponse>>
{
    private readonly ChangeMemberPositionUseCase _useCase;

    public ChangeMemberPositionHandler(ChangeMemberPositionUseCase useCase)
        => _useCase = useCase;

    public Task<Result<ChangeMemberPositionResponse>> Handle(
        ChangeMemberPositionCommand request, CancellationToken cancellationToken)
        => _useCase.HandleAsync(request, cancellationToken);
}
