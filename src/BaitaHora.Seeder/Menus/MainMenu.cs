using BaitaHora.Seeder.Cli.Core;
using BaitaHora.Seeder.Cli.Flows;
using BaitaHora.Seeder.Cli.Utils;

namespace BaitaHora.Seeder.Cli.Menus;

public sealed class MainMenu : IMenu
{
    public Task<IMenu?> ShowAsync()
    {
        RenderHeader();

        if (!SessionState.IsAuthenticated)
        {
            // MENU PÚBLICO (antes do login)
            Console.WriteLine(" Primeiro acesso:");
            Console.WriteLine(" 1) Login como OWNER (defaults)");
            Console.WriteLine(" 2) Login como EMPLOYEE (defaults)");
            Console.WriteLine(" 3) Registrar OWNER + COMPANY");
            Console.WriteLine(" 4) Aplicar token manual");
            Console.WriteLine(" 8) Trocar BaseUrl");
            Console.WriteLine(" 0) Sair");
            Console.Write("Opção: ");
            var op = (Console.ReadLine() ?? "").Trim();

            switch (op)
            {
                case "1":
                    return DoAsync(async () => { await LoginFlows.LoginOwnerAsync(); });
                case "2":
                    return DoAsync(async () => { await LoginFlows.LoginEmployeeAsync(); });
                case "3":
                    return DoAsync(async () => { await RegisterFlows.RegistrarOwnerAsync(); });
                case "4":
                    TokenFlows.ApplyTokenManual();
                    return Task.FromResult<IMenu?>(this);
                case "8":
                    UrlHelper.TrocarBaseUrl();
                    return Task.FromResult<IMenu?>(this);
                case "0":
                    return Task.FromResult<IMenu?>(null);
                default:
                    ConsoleHelper.Warn("Opção inválida.");
                    ConsoleHelper.Pause();
                    return Task.FromResult<IMenu?>(this);
            }
        }
        else
        {
            // MENU AUTENTICADO (com seções)
            Console.WriteLine(" Seções:");
            Console.WriteLine(" 1) Companhia");
            Console.WriteLine(" 2) Usuário");
            Console.WriteLine(" 3) Token / Sessão");
            Console.WriteLine(" 4) Configurações");
            Console.WriteLine(" 0) Sair");
            Console.Write("Opção: ");
            var op = (Console.ReadLine() ?? "").Trim();

            return Task.FromResult<IMenu?>(op switch
            {
                "1" => new CompanyMenu(),
                "2" => new UserMenu(),
                "3" => new TokenMenu(),
                "4" => new SettingsMenu(),
                "0" => null,
                _   => this
            });
        }
    }

    private static Task<IMenu?> DoAsync(Func<Task> action)
        => DoAsync(action, next: new MainMenu());

    private static async Task<IMenu?> DoAsync(Func<Task> action, IMenu next)
    {
        try { await action(); }
        catch (Exception ex)
        {
            ConsoleHelper.Error(ex.Message);
            ConsoleHelper.Pause();
        }
        return next;
    }

    private static void RenderHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('═', 56));
        Console.WriteLine(" BaitaHora • Seeder CLI");
        Console.WriteLine(new string('─', 56));
        Console.ResetColor();

        Console.Write(" BaseUrl: ");
        Console.WriteLine(SessionState.BaseUrl);

        if (SessionState.IsAuthenticated)
        {
            var tail = SessionState.Jwt!.Length >= 8 ? SessionState.Jwt[^8..] : SessionState.Jwt!;
            Console.WriteLine($" Sessão: {SessionState.Email ?? "Usuário autenticado"}  (…{tail})");
            Console.WriteLine($" Nível:  {(SessionState.Roles.Length == 0 ? "—" : string.Join(", ", SessionState.Roles))}");
            Console.WriteLine(new string('─', 56));
            Console.WriteLine(" Área do sistema liberada.");
        }
        else
        {
            Console.WriteLine(" Sessão: não autenticado");
            Console.WriteLine(" Nível:  —");
            Console.WriteLine(new string('─', 56));
            Console.WriteLine(" Primeiro acesso: faça login (Owner/Employee) ou registre Owner + Company.");
        }

        Console.WriteLine(new string('═', 56));
        Console.WriteLine();
    }
}
