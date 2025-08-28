using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Users.CreateUser;

public sealed class ToggleUserActiveHandler
    : IRequestHandler<ToggleUserActiveCommand, Result>
{
    private readonly ToggleUserActiveUseCase _useCase;
    public ToggleUserActiveHandler(ToggleUserActiveUseCase useCase) => _useCase = useCase;

    public Task<Result> Handle(ToggleUserActiveCommand req, CancellationToken ct)
        => _useCase.HandleAsync(req.UserId, req.IsActive, ct);
}