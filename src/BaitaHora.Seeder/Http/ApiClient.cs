using System.Net.Http.Json;
using System.Text.Json;

namespace BaitaHora.Seeder.Http;

public sealed class ApiClient
{
    private static readonly JsonSerializerOptions _jsonOpts = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public async Task<HttpResponseMessage> PostJsonAsync<TReq>(string baseUrl, string path, TReq payload, CancellationToken ct = default)
    {
        using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
        return await http.PostAsJsonAsync(path, payload, _jsonOpts, ct);
    }

    public async Task<(int Status, string Body)> PostAndReadAsync<TReq>(string baseUrl, string path, TReq payload, CancellationToken ct = default)
    {
        var resp = await PostJsonAsync(baseUrl, path, payload, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);
        return ((int)resp.StatusCode, body);
    }
}