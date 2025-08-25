using BaitaHora.Seeder.Http;

namespace BaitaHora.Seeder.Cli.Core;

public static class SessionState
{
    public static string BaseUrl { get; set; } = "http://localhost:5176";
    public static ApiClient Api { get; set; } = null!;

    public static string? Jwt { get; set; }
    public static string? Email { get; set; }
    public static string[] Roles { get; set; } = Array.Empty<string>();

    public static bool IsAuthenticated => !string.IsNullOrWhiteSpace(Jwt);

    public static void ApplyToken(string token, string? email, string[] roles)
    {
        Jwt = token;
        Email = email;
        Roles = roles;
        Api.SetBearer(token);
    }

    public static void ClearToken()
    {
        Jwt = null;
        Email = null;
        Roles = Array.Empty<string>();
        Api.ClearBearer();
    }
}