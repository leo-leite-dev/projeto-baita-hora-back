using System.Text.Json;

namespace BaitaHora.Seeder.Logging;

public sealed class UiLoggerConsole : IUiLogger
{
    public void Append(string message) => Console.WriteLine(message);

    public void AppendRawOrJson(string? body)
    {
        if (string.IsNullOrWhiteSpace(body)) return;
        var txt = body.Trim();
        if (txt.StartsWith("{") || txt.StartsWith("["))
        {
            try
            {
                using var doc = JsonDocument.Parse(txt);
                Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
                return;
            }
            catch { /* imprime cru */ }
        }
        Console.WriteLine(body);
    }
}
