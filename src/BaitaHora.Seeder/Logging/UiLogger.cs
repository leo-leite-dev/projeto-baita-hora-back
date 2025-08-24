using System.Text.Json;

namespace BaitaHora.Seeder.Logging;

public sealed class UiLogger
{
    private TextBox? _target;
    private readonly JsonSerializerOptions _jsonOpts = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public void Bind(TextBox target) => _target = target;

    public void Append(string message)
    {
        if (_target is null) return;
        var line = $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";
        _target.AppendText(line);
    }

    public void AppendRawOrJson(string raw)
    {
        if (_target is null) return;
        var text = TryPrettyJson(raw);
        _target.AppendText(text + Environment.NewLine);
    }

    public void Clear() => _target?.Clear();

    private string TryPrettyJson(string raw)
    {
        try
        {
            using var doc = JsonDocument.Parse(raw);
            return JsonSerializer.Serialize(doc.RootElement, _jsonOpts);
        }
        catch
        {
            return raw;
        }
    }
}