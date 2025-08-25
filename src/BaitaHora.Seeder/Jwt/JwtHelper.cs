using System.Text;
using System.Text.Json;

namespace BaitaHora.Seeder.Cli.Jwt;

public static class JwtHelper
{
    public static (string? email, string[] roles) ExtractIdentityFromJwt(string jwt)
    {
        var payload = JwtPart(jwt, 1);
        if (string.IsNullOrWhiteSpace(payload))
            return (null, Array.Empty<string>());

        try
        {
            using var doc = JsonDocument.Parse(payload);
            var root = doc.RootElement;

            string[] emailKeys =
            {
                "email","upn","unique_name","name","sub",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            };

            string[] roleSingleKeys =
            {
                "role","companyRole","bh:role",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            };

            string[] roleArrayKeys =
            {
                "roles","bh:roles",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            };

            string? email = TryGetStringByKeys(root, emailKeys);

            var roles = new List<string>();
            AddIfStringByKeys(roles, root, roleSingleKeys);
            AddIfArrayByKeys(roles, root, roleArrayKeys);

            return (email, roles.Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
        }
        catch
        {
            return (null, Array.Empty<string>());
        }
    }

    public static string? JwtPart(string jwt, int idx)
    {
        var parts = jwt.Split('.');
        if (parts.Length <= idx) return null;
        return Base64UrlDecodeToString(parts[idx]);
    }

    public static string Base64UrlDecodeToString(string input)
    {
        var s = input.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        var bytes = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(bytes);
    }

    public static string PrettyJson(string json)
    {
        try
        {
            using var j = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(j, new JsonSerializerOptions { WriteIndented = true });
        }
        catch { return json; }
    }

    private static string? TryGetStringByKeys(JsonElement root, params string[] keys)
    {
        foreach (var k in keys)
            if (root.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.String)
                return v.GetString();
        return null;
    }

    private static void AddIfStringByKeys(List<string> list, JsonElement root, params string[] keys)
    {
        foreach (var k in keys)
            if (root.TryGetProperty(k, out var v) && v.ValueKind == JsonValueKind.String)
            {
                var s = v.GetString();
                if (!string.IsNullOrWhiteSpace(s)) list.Add(s!);
            }
    }

    private static void AddIfArrayByKeys(List<string> list, JsonElement root, params string[] keys)
    {
        foreach (var k in keys)
            if (root.TryGetProperty(k, out var arr) && arr.ValueKind == JsonValueKind.Array)
                foreach (var it in arr.EnumerateArray())
                    if (it.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(it.GetString()))
                        list.Add(it.GetString()!);
    }
}