using BaitaHora.Application.IServices.Auth;

namespace BaitaHora.Api.Web.Cookies;

public interface IJwtCookieWriter
{
    void Write(HttpResponse response, JwtCookie cookie);
}

public sealed class JwtCookieWriter : IJwtCookieWriter
{
    public void Write(HttpResponse response, JwtCookie cookie)
    {
        if (cookie.ExpiresAtUtc <= DateTimeOffset.UtcNow || string.IsNullOrEmpty(cookie.Value))
        {
            response.Cookies.Delete(cookie.Name, new CookieOptions { Path = cookie.Path });
            return;
        }

        var opts = new CookieOptions
        {
            Expires  = cookie.ExpiresAtUtc,
            HttpOnly = cookie.HttpOnly,
            Secure   = cookie.Secure,
            Path     = cookie.Path,
            SameSite = cookie.SameSite switch
            {
                "Strict" => SameSiteMode.Strict,
                "None"   => SameSiteMode.None,
                _        => SameSiteMode.Lax
            }
        };

        response.Cookies.Append(cookie.Name, cookie.Value, opts);
    }
}