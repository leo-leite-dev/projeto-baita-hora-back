namespace BaitaHora.Application.IServices.Auth;

public sealed class JwtCookieFactory : IJwtCookieFactory
{
    private const string CookieName = "jwtToken";

    public JwtCookie CreateLoginCookie(string token, TimeSpan ttl)
        => new JwtCookie(
            Name: CookieName,
            Value: token,
            ExpiresAtUtc: DateTimeOffset.UtcNow.Add(ttl),
            HttpOnly: true,
            Secure: true,
            Path: "/",
            SameSite: "Lax"
        );

    public JwtCookie CreateLogoutCookie()
        => new JwtCookie(
            Name: CookieName,
            Value: string.Empty,
            ExpiresAtUtc: DateTimeOffset.UtcNow.AddDays(-1),
            HttpOnly: true,
            Secure: true,
            Path: "/",
            SameSite: "Lax"
        );
}