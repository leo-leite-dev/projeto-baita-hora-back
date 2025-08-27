using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Users.Commands;
using BaitaHora.Application.Features.Users.UseCases;
using MediatR;

namespace BaitaHora.Application.Features.Users.Handlers;

public sealed class ToggleUserActiveHandler
    : IRequestHandler<ToggleUserActiveCommand, Result>
{
    private readonly ToggleUserActiveUseCase _useCase;
    public ToggleUserActiveHandler(ToggleUserActiveUseCase useCase) => _useCase = useCase;

    public Task<Result> Handle(ToggleUserActiveCommand req, CancellationToken ct)
        => _useCase.HandleAsync(req.UserId, req.IsActive, ct);
}