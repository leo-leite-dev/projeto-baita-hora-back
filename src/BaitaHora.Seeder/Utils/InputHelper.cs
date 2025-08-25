namespace BaitaHora.Seeder.Cli.Utils;

public static class InputHelper
{
    public static Guid ReadGuid(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var raw = Console.ReadLine()?.Trim();
            if (Guid.TryParse(raw, out var id))
                return id;

            ConsoleHelper.Warn("GUID inválido. Tente novamente.");
        }
    }

    public static string ReadNonEmpty(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s))
                return s;
            ConsoleHelper.Warn("Valor obrigatório. Tente novamente.");
        }
    }
}