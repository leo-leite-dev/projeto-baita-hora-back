using BaitaHora.Seeder.Cli.Core;
using BaitaHora.Seeder.Cli.Utils;
using BaitaHora.Seeder.Builders;
using BaitaHora.Contracts.DTOs.Companies;
using BaitaHora.Contracts.Enums;

namespace BaitaHora.Seeder.Cli.Flows;

public static class RegisterFlows
{
    public static async Task RegistrarOwnerAsync()
    {
        var payload = PayloadBuilder.BuildRegisterOwnerWithCompany();
        ConsoleHelper.Info("POST /api/auth/register-owner");

        var (status, body) = await SessionState.Api.PostAndReadAsync(SessionState.BaseUrl, "/api/auth/register-owner", payload, default);
        Console.WriteLine($"HTTP {status}\n{body}");
        ConsoleHelper.Pause();
    }

    public static async Task RegistrarEmployeeAsync()
    {
        var companyId = InputHelper.ReadGuid("CompanyId (GUID): ");
        var positionId = InputHelper.ReadGuid("PositionId (GUID): ");

        var payload = PayloadBuilder.BuildRegisterEmployee(positionId);
        var path = $"/api/companies/{companyId:D}/employees";

        ConsoleHelper.Info($"POST {path}");
        var (status, body) = await SessionState.Api.PostAndReadAsync(SessionState.BaseUrl, path, payload, default);
        Console.WriteLine($"HTTP {status}\n{body}");
        ConsoleHelper.Pause();
    }

    public static async Task CriarPositionAsync()
    {
        var companyId = InputHelper.ReadGuid("CompanyId (GUID): ");
        var name = InputHelper.ReadNonEmpty("Nome da posição: ");

        Console.Write("Role (Owner/Manager/Staff/Viewer) [Staff]: ");
        var roleInput = Console.ReadLine();
        var role = Enum.TryParse<CompanyRole>(roleInput, true, out var parsed) ? parsed : CompanyRole.Staff;

        var req = new CreateCompanyPositionRequest(name.Trim(), role);
        var path = $"/api/companies/{companyId:D}/positions";

        ConsoleHelper.Info($"POST {path}");
        var (status, body) = await SessionState.Api.PostAndReadAsync(SessionState.BaseUrl, path, req, default);
        Console.WriteLine($"HTTP {status}\n{body}");
        ConsoleHelper.Pause();
    }
}
