namespace BaitaHora.Seeder.Cli.Menus;

public sealed class UserMenu : IMenu
{
    public Task<IMenu?> ShowAsync()
    {
        Console.WriteLine("== Usuário ==");
        Console.WriteLine(" 1) Owner");
        Console.WriteLine(" 2) Employee");
        Console.WriteLine(" 9) Voltar");
        Console.Write("Opção: ");

        var op = (Console.ReadLine() ?? "").Trim();
        return Task.FromResult<IMenu?>(op switch
        {
            "1" => new OwnerMenu(),
            "2" => new EmployeeMenu(),
            "9" => new MainMenu(),
            _   => this
        });
    }
}
