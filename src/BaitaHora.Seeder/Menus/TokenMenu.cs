using BaitaHora.Seeder.Cli.Flows;

namespace BaitaHora.Seeder.Cli.Menus;

public sealed class TokenMenu : IMenu
{
    public Task<IMenu?> ShowAsync()
    {
        Console.WriteLine("== Token / Sessão ==");
        Console.WriteLine(" 1) Aplicar token manual");
        Console.WriteLine(" 2) Limpar token");
        Console.WriteLine(" 3) Ver/decodificar JWT atual");
        Console.WriteLine(" 9) Voltar");
        Console.Write("Opção: ");
        var op = (Console.ReadLine() ?? "").Trim();

        switch (op)
        {
            case "1":
                TokenFlows.ApplyTokenManual();
                return Task.FromResult<IMenu?>(this);

            case "2":
                TokenFlows.ClearToken();
                return Task.FromResult<IMenu?>(this);

            case "3":
                TokenFlows.ShowJwtDetails();
                return Task.FromResult<IMenu?>(this);

            case "9":
            default:
                return Task.FromResult<IMenu?>(new MainMenu());
        }
    }
}
