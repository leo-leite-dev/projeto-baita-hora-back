using BaitaHora.Seeder.Builders;
using BaitaHora.Seeder.Http;
using BaitaHora.Seeder.Logging;

namespace BaitaHora.Seeder.Services;

public sealed class SeedService
{
    private readonly ApiClient _api;
    private readonly UiLogger _log;

    public SeedService(ApiClient api, UiLogger log)
    {
        _api = api;
        _log = log;
    }

    public async Task SeedOwnerAsync(string apiBaseUrl, CancellationToken ct = default)
    {
        _log.Append("Iniciando seed do Owner...");
        var payload = PayloadBuilder.BuildRegisterOwnerWithCompany();

        _log.Append("POST /api/auth/register-owner");
        var (status, body) = await _api.PostAndReadAsync(apiBaseUrl, "/api/auth/register-owner", payload, ct);

        _log.Append($"Status: {status}");
        _log.AppendRawOrJson(body);

        if (status is >= 200 and < 300)
            _log.Append("Owner registrado com sucesso.");
        else
            _log.Append("Falha ao registrar Owner.");
    }
}