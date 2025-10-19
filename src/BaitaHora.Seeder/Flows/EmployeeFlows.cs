using System.Text.Json;
using BaitaHora.Seeder.Cli.Core;
using BaitaHora.Seeder.Cli.Utils;

namespace BaitaHora.Seeder.Cli.Flows;

public static class EmployeeFlows
{
    public static async Task EditEmployeeAsync()
    {
        ConsoleHelper.Info("=== Edição de Employee ===");
        var companyId  = InputHelper.ReadGuid("CompanyId (GUID): ");
        var userId = InputHelper.ReadGuid("MemberId (GUID): ");

        Console.WriteLine();
        Console.WriteLine(" O que deseja editar?");
        Console.WriteLine(" 1) Trocar Position do funcionário");
        Console.WriteLine(" 2) Atualizar dados de perfil (nome/telefone)");
        Console.WriteLine(" 3) Ativar funcionário");
        Console.WriteLine(" 4) Desativar funcionário");
        Console.Write("Opção: ");
        var op = (Console.ReadLine() ?? "").Trim();

        try
        {
            switch (op)
            {
                case "1":
                    await UpdateEmployeePositionAsync(companyId, userId);
                    break;

                case "2":
                    await UpdateEmployeeProfileAsync(companyId, userId);
                    break;

                case "3":
                    await ActivateAsync(companyId, userId);
                    break;

                case "4":
                    await DeactivateAsync(companyId, userId);
                    break;

                default:
                    ConsoleHelper.Warn("Opção inválida.");
                    break;
            }
        }
        catch (Exception ex)
        {
            ConsoleHelper.Error(ex.Message);
        }

        ConsoleHelper.Pause();
    }

    private static async Task UpdateEmployeePositionAsync(Guid companyId, Guid userId)
    {
        var positionId = InputHelper.ReadGuid("Novo PositionId (GUID): ");

        // payload minimalista; ajuste se teu contrato exigir outro nome
        var payload = new
        {
            positionId
        };

        var path = $"/api/companies/{companyId:D}/employees/{userId:D}/Position";
        ConsoleHelper.Info($"PUT {path}");

        var (status, body) = await SessionState.Api.PutAndReadAsync(SessionState.BaseUrl, path, payload, default);
        Console.WriteLine($"HTTP {status}\n{body}");
    }

    private static async Task UpdateEmployeeProfileAsync(Guid companyId, Guid userId)
    {
        Console.Write("Nome completo (ENTER p/ manter): ");
        var fullName = Console.ReadLine();

        Console.Write("Telefone (ENTER p/ manter): ");
        var phone = Console.ReadLine();

        // Envia apenas o que o usuário preencheu (PATCH sem DTO fixo)
        var payload = BuildPartialPayload(
            ("fullName", fullName),
            ("phone",    phone)
        );

        if (payload.Count == 0)
        {
            ConsoleHelper.Warn("Nenhum campo informado. Nada a atualizar.");
            return;
        }

        var path = $"/api/companies/{companyId:D}/employees/{userId:D}";
        ConsoleHelper.Info($"PATCH {path}");

        var (status, body) = await SessionState.Api.PatchAndReadAsync(SessionState.BaseUrl, path, payload, default);
        Console.WriteLine($"HTTP {status}\n{body}");
    }

    private static async Task ActivateAsync(Guid companyId, Guid userId)
    {
        var path = $"/api/companies/{companyId:D}/employees/{userId:D}/activate";
        ConsoleHelper.Info($"POST {path}");

        var (status, body) = await SessionState.Api.PostAndReadAsync(SessionState.BaseUrl, path, new { }, default);
        Console.WriteLine($"HTTP {status}\n{body}");
    }

    private static async Task DeactivateAsync(Guid companyId, Guid userId)
    {
        var path = $"/api/companies/{companyId:D}/employees/{userId:D}/deactivate";
        ConsoleHelper.Info($"POST {path}");

        var (status, body) = await SessionState.Api.PostAndReadAsync(SessionState.BaseUrl, path, new { }, default);
        Console.WriteLine($"HTTP {status}\n{body}");
    }

    /// <summary>
    /// Monta um payload parcial (type: application/json) ignorando campos vazios.
    /// </summary>
    private static Dictionary<string, object> BuildPartialPayload(params (string key, string? value)[] pairs)
    {
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in pairs)
        {
            if (!string.IsNullOrWhiteSpace(value))
                dict[key] = value!.Trim();
        }
        return dict;
    }
}
