using BaitaHora.Seeder.Builders;
using BaitaHora.Seeder.Http;
using BaitaHora.Seeder.Logging;

namespace BaitaHora.Seeder.Services;

public sealed class SeedService
{
    private readonly ApiClient _api;
    private readonly IUiLogger _log;

    public SeedService(ApiClient api, IUiLogger log)
    {
        _api = api;
        _log = log;
    }

    public async Task<(int status, string? body)> SeedOwnerAsync(
        string apiBaseUrl,
        CancellationToken ct = default)
    {
        var payload = PayloadBuilder.BuildRegisterOwnerWithCompany();

        _log.Append("POST /api/auth/register-owner");
        var (status, body) = await _api.PostAndReadAsync(
            apiBaseUrl,
            "/api/auth/register-owner",
            payload,
            ct);

        _log.Append($"Status: {status}");
        _log.AppendRawOrJson(body);

        return (status, body);
    }

    public async Task<(int status, string? body)> SeedEmployeeAsync(
        string apiBaseUrl,
        Guid positionId,
        CancellationToken ct = default)
    {
        _log.Append("Iniciando seed do Employee...");

        var payload = PayloadBuilder.BuildRegisterEmployee(positionId);

        _log.Append("POST /api/auth/register-employee");
        var (status, body) = await _api.PostAndReadAsync(
            apiBaseUrl,
            "/api/auth/register-employee",
            payload,
            ct);

        _log.Append($"Status: {status}");
        _log.AppendRawOrJson(body);

        return (status, body);
    }
}