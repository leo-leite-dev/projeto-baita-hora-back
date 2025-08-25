using BaitaHora.Seeder.Cli.Flows;

namespace BaitaHora.Seeder.Cli.Menus;

public sealed class OwnerMenu : IMenu
{
    public async Task<IMenu?> ShowAsync()
    {
        Console.WriteLine("== Usuário • Owner ==");
        Console.WriteLine(" 1) Login como OWNER (defaults)");
        Console.WriteLine(" 9) Voltar");
        Console.Write("Opção: ");
        var op = (Console.ReadLine() ?? "").Trim();

        switch (op)
        {
            case "1":
                await LoginFlows.LoginOwnerAsync();
                return this;
            case "9":
            default:
                return new UserMenu();
        }
    }
}
