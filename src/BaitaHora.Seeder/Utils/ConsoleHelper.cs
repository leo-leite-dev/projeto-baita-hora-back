namespace BaitaHora.Seeder.Cli.Utils;

public static class ConsoleHelper
{
    public static void Ok(string msg)   { Console.ForegroundColor = ConsoleColor.Green;  Console.WriteLine(msg); Console.ResetColor(); }
    public static void Warn(string msg) { Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(msg); Console.ResetColor(); }
    public static void Error(string msg){ Console.ForegroundColor = ConsoleColor.Red;    Console.WriteLine("[ERRO] " + msg); Console.ResetColor(); }
    public static void Info(string msg) { Console.ForegroundColor = ConsoleColor.Cyan;   Console.WriteLine(msg); Console.ResetColor(); }

    public static void Pause()
    {
        Console.WriteLine();
        Console.Write("ENTER para continuar...");
        Console.ReadLine();
    }
}