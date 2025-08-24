using BaitaHora.Seeder.Config;
using BaitaHora.Seeder.Http;
using BaitaHora.Seeder.Logging;
using BaitaHora.Seeder.Presentation.Forms;
using BaitaHora.Seeder.Services;

namespace BaitaHora.Seeder;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var cfg = AppConfig.Load();

        var apiClient = new ApiClient();
        var logger = new UiLogger();

        var seedService = new SeedService(apiClient, logger);

        var form = new MainForm(cfg, seedService, logger);
        Application.Run(form);
    }
}
