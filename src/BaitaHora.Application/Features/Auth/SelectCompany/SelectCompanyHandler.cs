using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Auth.Login;
using MediatR;

namespace BaitaHora.Application.Features.Auth.SelectCompany;

public sealed class SelectCompanyHandler
    : IRequestHandler<SelectCompanyCommand, Result<AuthResult>>
{
    private readonly ICurrentUser _current;
    private readonly SelectCompanyUseCase _uc;

    public SelectCompanyHandler(ICurrentUser current, SelectCompanyUseCase uc)
    {
        _current = current;
        _uc = uc;
    }

    public Task<Result<AuthResult>> Handle(SelectCompanyCommand cmd, CancellationToken ct)
        => _uc.HandleAsync(_current.UserId, _current.Username, cmd, ct);
}