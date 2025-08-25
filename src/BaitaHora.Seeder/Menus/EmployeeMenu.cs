using BaitaHora.Seeder.Cli.Flows;

namespace BaitaHora.Seeder.Cli.Menus;

public sealed class EmployeeMenu : IMenu
{
    public async Task<IMenu?> ShowAsync()
    {
        Console.WriteLine("== Usuário • Employee ==");
        Console.WriteLine(" 1) Registrar EMPLOYEE");
        Console.WriteLine(" 2) Trocar Position");
        Console.WriteLine(" 3) Atualizar Perfil");
        Console.WriteLine(" 4) Ativar");
        Console.WriteLine(" 5) Desativar");
        Console.WriteLine(" 6) Remover EMPLOYEE");
        Console.WriteLine(" 7) Login como EMPLOYEE (defaults)");
        Console.WriteLine(" 9) Voltar");
        Console.Write("Opção: ");
        var op = (Console.ReadLine() ?? "").Trim();

        switch (op)
        {
            case "1": await RegisterFlows.RegistrarEmployeeAsync(); break;
            case "2": await EmployeeEditFlows.AlterarPositionAsync(); break;
            case "3": await EmployeeEditFlows.AtualizarPerfilAsync(); break;
            case "4": await EmployeeEditFlows.AtivarAsync(); break;
            case "5": await EmployeeEditFlows.DesativarAsync(); break;
            case "6": await EmployeeEditFlows.RemoverAsync(); break;
            case "7": await LoginFlows.LoginEmployeeAsync(); break;
            case "9": return new UserMenu();
            default: break;
        }
        return this;
    }
}
