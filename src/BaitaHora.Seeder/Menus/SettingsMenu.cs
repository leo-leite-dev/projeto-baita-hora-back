using BaitaHora.Seeder.Cli.Utils;

namespace BaitaHora.Seeder.Cli.Menus;

public sealed class SettingsMenu : IMenu
{
    public Task<IMenu?> ShowAsync()
    {
        Console.WriteLine("== Configurações ==");
        Console.WriteLine(" 1) Trocar BaseUrl");
        Console.WriteLine(" 9) Voltar");
        Console.Write("Opção: ");
        var op = (Console.ReadLine() ?? "").Trim();

        switch (op)
        {
            case "1":
                UrlHelper.TrocarBaseUrl();
                return Task.FromResult<IMenu?>(this);

            case "9":
            default:
                return Task.FromResult<IMenu?>(new MainMenu());
        }
    }
}