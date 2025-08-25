using System.Text.Json;
using BaitaHora.Seeder.Cli.Core;
using BaitaHora.Seeder.Cli.Jwt;
using BaitaHora.Seeder.Cli.Utils;

namespace BaitaHora.Seeder.Cli.Flows;

public static class TokenFlows
{
    public static bool IsAuthenticated() => SessionState.IsAuthenticated;

    public static void TryCaptureToken(string? body)
    {
        if (string.IsNullOrWhiteSpace(body)) return;

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("accessToken", out var t) || t.ValueKind != JsonValueKind.String)
                return;

            var token = t.GetString();
            if (string.IsNullOrWhiteSpace(token)) return;

            var (email, roles) = JwtHelper.ExtractIdentityFromJwt(token!);
            SessionState.ApplyToken(token!, email, roles);
            ConsoleHelper.Ok("Token aplicado. Sessão iniciada.");
        }
        catch
        {
            // silencioso
        }
    }

    public static void ApplyTokenManual()
    {
        Console.Write("Cole o JWT: ");
        var token = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            ConsoleHelper.Warn("Token vazio.");
            ConsoleHelper.Pause();
            return;
        }

        var (email, roles) = JwtHelper.ExtractIdentityFromJwt(token);
        SessionState.ApplyToken(token, email, roles);
        ConsoleHelper.Ok("Token aplicado.");
        ConsoleHelper.Pause();
    }

    public static void ClearToken()
    {
        SessionState.ClearToken();
        ConsoleHelper.Warn("Sessão encerrada. Token limpo.");
        ConsoleHelper.Pause();
    }

    public static void ShowJwtDetails()
    {
        if (!IsAuthenticated())
        {
            ConsoleHelper.Warn("Sem JWT aplicado.");
            ConsoleHelper.Pause();
            return;
        }

        var (hdr, payload) = (JwtHelper.JwtPart(SessionState.Jwt!, 0), JwtHelper.JwtPart(SessionState.Jwt!, 1));
        Console.WriteLine("\nHeader:");
        Console.WriteLine(JwtHelper.PrettyJson(hdr ?? "{}"));
        Console.WriteLine("\nPayload:");
        Console.WriteLine(JwtHelper.PrettyJson(payload ?? "{}"));
        Console.WriteLine("\nAssinatura: (oculta)");
        ConsoleHelper.Pause();
    }
}
