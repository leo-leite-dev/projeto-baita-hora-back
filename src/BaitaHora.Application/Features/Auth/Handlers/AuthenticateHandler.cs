using BaitaHora.Application.Auth.Commands;
using BaitaHora.Application.Auth.DTOs.Responses;
using BaitaHora.Application.Auth.UseCases.Authenticate;
using BaitaHora.Application.Common;
using MediatR;

namespace BaitaHora.Application.Auth.Handlers;

public sealed class AuthenticateHandler : IRequestHandler<AuthenticateCommand, Result<AuthTokenResponse>>
{
    private readonly AuthenticateUseCase _uc;
    public AuthenticateHandler(AuthenticateUseCase uc) => _uc = uc;
    public Task<Result<AuthTokenResponse>> Handle(AuthenticateCommand cmd, CancellationToken ct)
        => _uc.HandleAsync(cmd.Input, ct);
}