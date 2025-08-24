using BaitaHora.Seeder.Config;
using BaitaHora.Seeder.Logging;
using BaitaHora.Seeder.Services;

namespace BaitaHora.Seeder.Presentation.Forms;

public sealed class MainForm : Form
{
    private readonly AppConfig _cfg;
    private readonly SeedService _seedService;
    private readonly UiLogger _logger;

    // UI
    private TextBox _txtApi = default!;
    private Button _btnSalvar = default!;
    private Button _btnClear = default!;
    private TextBox _txtLog = default!;

    public MainForm(AppConfig cfg, SeedService seedService, UiLogger logger)
    {
        _cfg = cfg;
        _seedService = seedService;
        _logger = logger;

        Text = "BaitaHora • Seed de Owner";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 760;
        Height = 520;

        InitUi();
        WireEvents();
    }

    private void InitUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 4,
            Padding = new Padding(12)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        // Linha 0: API Base URL
        var lblApi = new Label { Text = "API Base URL:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        _txtApi = new TextBox { Dock = DockStyle.Fill, Text = _cfg.ApiBaseUrl };

        // Linha 1: botões
        var pnlButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        _btnSalvar = new Button { Text = "Salvar Owner", AutoSize = true };
        _btnClear  = new Button { Text = "Limpar Logs", AutoSize = true };
        pnlButtons.Controls.AddRange(new Control[] { _btnSalvar, _btnClear });

        // Linha 2-3: Logs
        var lblLogs = new Label { Text = "Logs:", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        _txtLog = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true };

        // Add
        root.Controls.Add(lblApi, 0, 0);
        root.Controls.Add(_txtApi, 1, 0);
        root.Controls.Add(new Panel(), 0, 1);
        root.Controls.Add(pnlButtons, 1, 1);
        root.Controls.Add(lblLogs, 0, 2);
        root.Controls.Add(_txtLog, 1, 2);
        root.SetRowSpan(_txtLog, 2);

        Controls.Add(root);

        // Logger aponta para o TextBox
        _logger.Bind(_txtLog);
    }

    private void WireEvents()
    {
        _btnSalvar.Click += async (_, __) => await OnSalvarOwnerAsync();
        _btnClear.Click  += (_, __) => _logger.Clear();
    }

    private async Task OnSalvarOwnerAsync()
    {
        ToggleUi(false);

        try
        {
            await _seedService.SeedOwnerAsync(_txtApi.Text);
            MessageBox.Show("Operação concluída. Verifique os logs.", "Seeder", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.Append("Erro ao executar seed:");
            _logger.Append(ex.ToString());
            MessageBox.Show("Erro ao chamar a API. Veja o log.", "Seeder", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            ToggleUi(true);
        }
    }

    private void ToggleUi(bool enabled)
    {
        _btnSalvar.Enabled = enabled;
        _btnClear.Enabled  = enabled;
        _txtApi.Enabled    = enabled;
    }
}