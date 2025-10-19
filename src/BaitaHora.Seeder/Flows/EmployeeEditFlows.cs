using BaitaHora.Seeder.Cli.Core;
using BaitaHora.Seeder.Cli.Utils;

namespace BaitaHora.Seeder.Cli.Flows;

public static class EmployeeEditFlows
{
    // Trocar posição (Position) de um funcionário
    public static async Task AlterarPositionAsync()
    {
        var companyId  = InputHelper.ReadGuid("CompanyId (GUID): ");
        var userId = InputHelper.ReadGuid("MemberId (GUID): ");
        var positionId = InputHelper.ReadGuid("Novo PositionId (GUID): ");

        var payload = new { positionId };
        var path = $"/api/companies/{companyId:D}/employees/{userId:D}/Position";

        ConsoleHelper.Info($"PUT {path}");
        var (status, body) = await SessionState.Api.PutAndReadAsync(SessionState.BaseUrl, path, payload, default);

        Console.WriteLine($"HTTP {status}\n{body}");
        ConsoleHelper.Pause();
    }

    // Atualizar dados do perfil do funcionário
    public static async Task AtualizarPerfilAsync()
    {
        var companyId  = InputHelper.ReadGuid("CompanyId (GUID): ");
        var userId = InputHelper.ReadGuid("MemberId (GUID): ");

        Console.Write("Nome completo (ENTER p/ manter): ");
        var fullName = Console.ReadLine();

        Console.Write("Telefone (ENTER p/ manter): ");
        var phone = Console.ReadLine();

        var payload = BuildPartialPayload(("fullName", fullName), ("phone", phone));
        if (payload.Count == 0)
        {
            ConsoleHelper.Warn("Nenhum campo informado. Nada a atualizar.");
            ConsoleHelper.Pause();
            return;
        }

        var path = $"/api/companies/{companyId:D}/employees/{userId:D}";
        ConsoleHelper.Info($"PATCH {path}");
        var (status, body) = await SessionState.Api.PatchAndReadAsync(SessionState.BaseUrl, path, payload, default);

        Console.WriteLine($"HTTP {status}\n{body}");
        ConsoleHelper.Pause();
    }

    // Ativar funcionário
    public static async Task AtivarAsync()
    {
        var companyId  = InputHelper.ReadGuid("CompanyId (GUID): ");
        var userId = InputHelper.ReadGuid("MemberId (GUID): ");

        var path = $"/api/companies/{companyId:D}/employees/{userId:D}/activate";

        ConsoleHelper.Info($"POST {path}");
        var (status, body) = await SessionState.Api.PostAndReadAsync(SessionState.BaseUrl, path, new { }, default);

        Console.WriteLine($"HTTP {status}\n{body}");
        ConsoleHelper.Pause();
    }

    // Desativar funcionário
    public static async Task DesativarAsync()
    {
        var companyId  = InputHelper.ReadGuid("CompanyId (GUID): ");
        var userId = InputHelper.ReadGuid("MemberId (GUID): ");

        var path = $"/api/companies/{companyId:D}/employees/{userId:D}/deactivate";

        ConsoleHelper.Info($"POST {path}");
        var (status, body) = await SessionState.Api.PostAndReadAsync(SessionState.BaseUrl, path, new { }, default);

        Console.WriteLine($"HTTP {status}\n{body}");
        ConsoleHelper.Pause();
    }

    // Remover funcionário
    public static async Task RemoverAsync()
    {
        var companyId  = InputHelper.ReadGuid("CompanyId (GUID): ");
        var userId = InputHelper.ReadGuid("MemberId (GUID): ");

        var path = $"/api/companies/{companyId:D}/employees/{userId:D}";

        ConsoleHelper.Info($"DELETE {path}");
        var (status, body) = await SessionState.Api.DeleteAndReadAsync(SessionState.BaseUrl, path, default);

        Console.WriteLine($"HTTP {status}\n{body}");
        ConsoleHelper.Pause();
    }

    // helper pra PATCH parcial
    private static Dictionary<string, object> BuildPartialPayload(params (string key, string? value)[] pairs)
    {
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in pairs)
            if (!string.IsNullOrWhiteSpace(value))
                dict[key] = value!.Trim();
        return dict;
    }
}
