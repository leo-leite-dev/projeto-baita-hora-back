namespace BaitaHora.Application.IServices.Auth;

public sealed class JwtCookieOptions
{
    public string Name { get; set; } = "jwtToken";
    public bool HttpOnly { get; set; } = true;
    public bool Secure { get; set; } = true;
    public string Path { get; set; } = "/";
    public string SameSite { get; set; } = "Lax";
}