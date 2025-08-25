using BaitaHora.Seeder.Cli.Core;

namespace BaitaHora.Seeder.Cli.Utils;

public static class UrlHelper
{
    public static void TrocarBaseUrl()
    {
        Console.Write("Nova BaseUrl: ");
        var next = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(next))
        {
            ConsoleHelper.Warn("BaseUrl não alterada.");
            ConsoleHelper.Pause();
            return;
        }

        try { SessionState.Api.Dispose(); } catch { /* ignore */ }

        SessionState.BaseUrl = next!;
        SessionState.Api = new BaitaHora.Seeder.Http.ApiClient(SessionState.BaseUrl);

        if (SessionState.IsAuthenticated && SessionState.Jwt is { } jwt)
            SessionState.Api.SetBearer(jwt);

        ConsoleHelper.Ok("BaseUrl atualizada.");
        ConsoleHelper.Pause();
    }
}