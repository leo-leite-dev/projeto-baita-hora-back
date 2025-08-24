using System.Text.Json;

namespace BaitaHora.Seeder.Config;

public sealed class AppConfig
{
    public string ApiBaseUrl { get; set; } = "http://localhost:5176";

    public static AppConfig Load()
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path)) return new AppConfig();

            var json = File.ReadAllText(path);
            var cfg = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return cfg ?? new AppConfig();
        }
        catch
        {
            return new AppConfig();
        }
    }
}