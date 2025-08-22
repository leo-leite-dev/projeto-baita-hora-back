using BaitaHora.Api.Contracts.Auth.Requests;
using BaitaHora.Application.Features.Auth.Commands;

namespace BaitaHora.Api.Contracts.Auth.Mappers;

public static class AuthApiMappers
{
    public static AuthenticateCommand ToCommand(this AuthenticateRequest r)
        => new()
        {
            Identify    = r.Identify,
            RawPassword = r.Password
        };
}