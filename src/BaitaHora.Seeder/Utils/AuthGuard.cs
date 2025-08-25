using BaitaHora.Seeder.Cli.Core;
using BaitaHora.Seeder.Cli.Utils;

namespace BaitaHora.Seeder.Cli.Menus;

public static class AuthGuard
{
    public static bool EnsureAuthenticated()
    {
        if (SessionState.IsAuthenticated) return true;

        ConsoleHelper.Warn("É necessário estar autenticado para acessar esta seção.");
        ConsoleHelper.Pause();
        return false;
    }
}