using BaitaHora.Seeder.Cli.Core;
using BaitaHora.Seeder.Cli.Utils;
using BaitaHora.Seeder.Cli.Flows;
using BaitaHora.Seeder.Builders;

namespace BaitaHora.Seeder.Cli.Flows;

public static class LoginFlows
{
    public static async Task LoginOwnerAsync()
    {
        var owner = OwnerBuilder.Default();
        await DoLoginAsync(owner.Email, owner.RawPassword);
    }

    public static async Task LoginEmployeeAsync()
    {
        var emp = EmployeeBuilder.Default();
        await DoLoginAsync(emp.Email, emp.RawPassword);
    }

    private static async Task DoLoginAsync(string identify, string password)
    {
        ConsoleHelper.Info($"POST /api/auth/login → {identify}");
        var (status, body) = await SessionState.Api.LoginAsync(identify, password, default);

        Console.WriteLine($"HTTP {status}");
        Console.WriteLine(body);

        TokenFlows.TryCaptureToken(body);
        ConsoleHelper.Pause();
    }
}
