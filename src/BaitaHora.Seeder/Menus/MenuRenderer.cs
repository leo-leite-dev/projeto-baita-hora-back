namespace BaitaHora.Seeder.Cli.Menus;

public static class MenuRenderer
{
    public static void RenderHeader(string baseUrl, string? jwt, string? email, string[] roles)
    {
        Line('═', 56, ConsoleColor.Cyan);
        Title("BaitaHora • Seeder CLI", ConsoleColor.Cyan);
        Line('─', 56, ConsoleColor.DarkCyan);

        Label("BaseUrl", baseUrl);

        if (!string.IsNullOrWhiteSpace(jwt))
        {
            var who = string.IsNullOrWhiteSpace(email) ? "Usuário autenticado" : email!;
            var tail = jwt.Length >= 8 ? jwt[^8..] : jwt;
            Label("Sessão", $"{who}  (…{tail})", ConsoleColor.Green);

            var nivel = roles.Length == 0 ? "—" : string.Join(", ", roles);
            Label("Nível de acesso", nivel, ConsoleColor.Green);
        }
        else
        {
            Label("Sessão", "não autenticado", ConsoleColor.Yellow);
            Label("Nível de acesso", "—", ConsoleColor.Yellow);
        }

        Line('─', 56, ConsoleColor.DarkCyan);
        Console.WriteLine(!string.IsNullOrWhiteSpace(jwt)
            ? "Área do sistema liberada."
            : "Primeiro acesso: faça login (Owner/Employee) ou registre Owner + Company.");
        Line('═', 56, ConsoleColor.Cyan);
        Console.WriteLine();
    }

    public static void RenderPublicMenu()
    {
        Console.WriteLine(" 1) Login como OWNER (defaults)");
        Console.WriteLine(" 2) Login como EMPLOYEE (defaults)");
        Console.WriteLine(" 3) Registrar OWNER + COMPANY");
        Console.WriteLine(" 8) Trocar BaseUrl");
        Console.WriteLine(" 0) Sair");
    }

    public static void RenderPrivateMenu()
    {
        Console.WriteLine(" 1) Login como OWNER (defaults)");
        Console.WriteLine(" 2) Login como EMPLOYEE (defaults)");
        Console.WriteLine(" 3) Aplicar token manual");
        Console.WriteLine(" 4) Limpar token");
        Console.WriteLine(" 5) Registrar OWNER + COMPANY");
        Console.WriteLine(" 6) Registrar EMPLOYEE");
        Console.WriteLine(" 7) Criar POSITION (nome digitado)");
        Console.WriteLine(" 9) Ver/decodificar JWT atual");
        Console.WriteLine(" 8) Trocar BaseUrl");
        Console.WriteLine("10) Editar EMPLOYEE");
        Console.WriteLine(" 0) Sair");
    }

    private static void Title(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($" {text}");
        Console.ResetColor();
    }

    private static void Label(string key, string value, ConsoleColor? color = null)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($" {key}: ");
        Console.ResetColor();
        if (color is { } c) Console.ForegroundColor = c;
        Console.WriteLine(value);
        Console.ResetColor();
    }

    private static void Line(char ch, int len, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(new string(ch, len));
        Console.ResetColor();
    }
}