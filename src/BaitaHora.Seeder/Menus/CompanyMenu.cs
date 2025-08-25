using BaitaHora.Seeder.Cli.Flows;

namespace BaitaHora.Seeder.Cli.Menus;

public sealed class CompanyMenu : IMenu
{
    public async Task<IMenu?> ShowAsync()
    {
        if (!AuthGuard.EnsureAuthenticated()) return new MainMenu();

        Console.WriteLine("== Companhia ==");
        Console.WriteLine(" 1) Registrar OWNER + COMPANY");
        Console.WriteLine(" 2) Editar COMPANY (renomear)");
        Console.WriteLine(" 3) Criar POSITION");
        Console.WriteLine(" 9) Voltar");
        Console.Write("Opção: ");
        var op = (Console.ReadLine() ?? "").Trim();

        switch (op)
        {
            case "1": await RegisterFlows.RegistrarOwnerAsync(); return this;
            case "2": await CompanyEditFlows.RenomearCompanyAsync(); return this;
            case "3": await RegisterFlows.CriarPositionAsync(); return this;
            case "9":
            default:  return new MainMenu();
        }
    }
}
