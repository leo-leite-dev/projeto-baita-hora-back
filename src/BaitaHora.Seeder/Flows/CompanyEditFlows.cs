using BaitaHora.Seeder.Cli.Core;
using BaitaHora.Seeder.Cli.Utils;

namespace BaitaHora.Seeder.Cli.Flows;

public static class CompanyEditFlows
{
    public static async Task RenomearCompanyAsync()
    {
        var companyId = InputHelper.ReadGuid("CompanyId (GUID): ");
        var name      = InputHelper.ReadNonEmpty("Novo nome da empresa: ");

        var payload = new { name };
        var path = $"/api/companies/{companyId:D}";

        ConsoleHelper.Info($"PATCH {path}");
        var (status, body) = await SessionState.Api.PatchAndReadAsync(SessionState.BaseUrl, path, payload, default);
        Console.WriteLine($"HTTP {status}\n{body}");
        ConsoleHelper.Pause();
    }
}