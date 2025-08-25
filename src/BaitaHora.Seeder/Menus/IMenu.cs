namespace BaitaHora.Seeder.Cli.Menus;

public interface IMenu
{
    Task<IMenu?> ShowAsync();
}