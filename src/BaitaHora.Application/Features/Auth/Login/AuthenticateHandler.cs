using BaitaHora.Application.Common.Results;
using MediatR;

namespace BaitaHora.Application.Features.Auth.Login;

public sealed class AuthenticateHandler : IRequestHandler<AuthenticateCommand, Result<AuthResult>>
{
    private readonly AuthenticateUseCase _uc;
    public AuthenticateHandler(AuthenticateUseCase uc) => _uc = uc;
    public Task<Result<AuthResult>> Handle(AuthenticateCommand cmd, CancellationToken ct)
        => _uc.HandleAsync(cmd, ct);
}