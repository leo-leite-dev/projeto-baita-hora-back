namespace BaitaHora.Application.IServices.Auth;

public interface IJwtCookieFactory
{
    JwtCookie CreateLoginCookie(string token, TimeSpan ttl);
    JwtCookie CreateLogoutCookie();
}