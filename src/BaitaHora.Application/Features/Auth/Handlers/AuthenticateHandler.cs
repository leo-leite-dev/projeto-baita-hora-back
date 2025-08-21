using BaitaHora.Application.Common;
using BaitaHora.Application.Feature.Auth.DTOs.Responses;
using BaitaHora.Application.Features.Auth.Commands;
using BaitaHora.Application.Features.Auth.UseCases;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Handlers;

public sealed class AuthenticateHandler : IRequestHandler<AuthenticateCommand, Result<AuthTokenResponse>>
{
    private readonly AuthenticateUseCase _uc;
    public AuthenticateHandler(AuthenticateUseCase uc) => _uc = uc;
    public Task<Result<AuthTokenResponse>> Handle(AuthenticateCommand cmd, CancellationToken ct)
        => _uc.HandleAsync(cmd.Input, ct);
}