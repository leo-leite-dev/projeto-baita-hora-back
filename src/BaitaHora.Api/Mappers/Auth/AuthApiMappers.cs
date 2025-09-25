using BaitaHora.Application.Features.Auth.Login;
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

    public static AuthResponse ToResponse(this AuthResult result)
        => new(
            result.AccessToken,
            result.RefreshToken,
            result.ExpiresAtUtc,
            result.UserId,
            result.Username.Value,
            result.Roles.Select(r => r.ToString()).ToList(),
            result.Companies.Select(c => new AuthCompanyResponse(
                c.CompanyId,
                c.Name
            )).ToList()
        );
}
