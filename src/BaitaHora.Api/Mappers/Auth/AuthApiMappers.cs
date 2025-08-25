using BaitaHora.Application.Features.Auth.Commands;
using BaitaHora.Contracts.DTOS.Auth;

namespace BaitaHora.Api.Mappers.Auth;

public static class AuthApiMappers
{
    public static AuthenticateCommand ToCommand(this AuthenticateRequest r)
        => new()
        {
            Identify = r.Identify,
            RawPassword = r.Password
        };
}
