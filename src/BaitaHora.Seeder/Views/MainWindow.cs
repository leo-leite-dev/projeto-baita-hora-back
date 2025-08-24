using Avalonia.Controls;
using Avalonia.Interactivity;
using BaitaHora.Seeder.Http;
using BaitaHora.Seeder.Logging;
using BaitaHora.Seeder.Models;
using BaitaHora.Seeder.Services;

namespace BaitaHora.Seeder.Gui;

public partial class MainWindow : Window
{
    private readonly SeedService _seed;
    private const string ApiBase = "http://localhost:5176";

    public MainWindow()
    {
        InitializeComponent();

        UserTypeCombo!.ItemsSource = Enum.GetValues(typeof(SeedUserType));
        UserTypeCombo.SelectedItem = SeedUserType.OwnerWithCompany;

        var api = new ApiClient();
        var log = new UiLoggerConsole();
        _seed = new SeedService(api, log);

        RunBtn!.AddHandler(Button.ClickEvent, OnRunClick);
    }

    private async void OnRunClick(object? sender, RoutedEventArgs e)
    {
        RunBtn!.IsEnabled = false;
        StatusBlock!.Text = "Executando...";

        var type = (SeedUserType)(UserTypeCombo!.SelectedItem ?? SeedUserType.OwnerWithCompany);

        try
        {
            switch (type)
            {
                case SeedUserType.OwnerWithCompany:
                {
                    var (status, body) = await _seed.SeedOwnerAsync(ApiBase, CancellationToken.None);
                    StatusBlock.Text = body ?? $"HTTP {status}";
                    break;
                }

                case SeedUserType.Employee:
                {
                    var txt = PositionIdBox!.Text?.Trim();
                    if (!Guid.TryParse(txt, out var positionId))
                    {
                        StatusBlock.Text = "PositionId inválido (GUID).";
                        return;
                    }

                    var (status, body) = await _seed.SeedEmployeeAsync(ApiBase, positionId, CancellationToken.None);
                    StatusBlock.Text = body ?? $"HTTP {status}";
                    break;
                }

                default:
                    StatusBlock.Text = "Seleção inválida.";
                    break;
            }
        }
        catch (Exception ex)
        {
            StatusBlock.Text = $"Erro: {ex.Message}";
        }
        finally
        {
            RunBtn.IsEnabled = true;
        }
    }
}