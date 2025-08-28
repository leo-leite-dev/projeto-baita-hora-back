using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Auth;

public sealed class AuthenticateHandler : IRequestHandler<AuthenticateCommand, Result<AuthTokenResponse>>
{
    private readonly AuthenticateUseCase _uc;
    public AuthenticateHandler(AuthenticateUseCase uc) => _uc = uc;
    public Task<Result<AuthTokenResponse>> Handle(AuthenticateCommand cmd, CancellationToken ct)
        => _uc.HandleAsync(cmd, ct);
}