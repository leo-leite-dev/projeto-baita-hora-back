namespace BaitaHora.Application.Abstractions.Auth;

public interface IJwtCookieFactory
{
    JwtCookie CreateLoginCookie(string token, TimeSpan ttl);
    JwtCookie CreateLogoutCookie();
}