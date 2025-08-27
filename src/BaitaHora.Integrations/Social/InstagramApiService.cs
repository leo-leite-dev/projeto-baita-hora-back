using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BaitaHora.Application.Abstractions.Integrations;
using BaitaHora.Integrations.Social;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class InstagramApi : IInstagramApi
{
    private readonly HttpClient _http;
    private readonly InstagramOptions _opt;
    private readonly ILogger<InstagramApi> _logger;

    public InstagramApi(HttpClient http, IOptions<InstagramOptions> opt, ILogger<InstagramApi> logger)
    {
        _http = http;
        _opt = opt.Value;
        _logger = logger;
    }

    public async Task SendTextAsync(string igBusinessId, string recipientPsid, string text, CancellationToken ct = default)
    {
        Guard();

        var url = $"{igBusinessId}/messages?access_token={_opt.AccessToken}";

        var payload = new
        {
            messaging_product = "instagram",
            recipient = new { id = recipientPsid },
            message = new { text }
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload)
        };

        var res = await _http.SendAsync(req, ct);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Graph error {(int)res.StatusCode}: {body}");
        }
    }


    public async Task DiagnoseAsync(string pageAccessToken, string appId, string appSecret, string igUserId, CancellationToken ct = default)
    {
        // 1) Debug do token
        var debug = await _http.GetFromJsonAsync<JsonElement>(
            $"debug_token?input_token={Uri.EscapeDataString(pageAccessToken)}&access_token={appId}|{appSecret}", ct);

        Console.WriteLine("[DIAG] debug_token: " + debug);

        // 2) Info do IG user
        var igInfo = await _http.GetFromJsonAsync<JsonElement>(
            $"{igUserId}?fields=id,username,ig_id,name", ct);

        Console.WriteLine("[DIAG] ig user: " + igInfo);
    }

    private void Guard()
    {
        if (string.IsNullOrWhiteSpace(_opt.AccessToken))
            throw new InvalidOperationException("Instagram AccessToken não configurado.");
        if (string.IsNullOrWhiteSpace(_opt.InstagramUserId))
            throw new InvalidOperationException("InstagramUserId (IG Business Account ID) não configurado.");
    }
}

public sealed record GraphAppDiagnostics
{
    public string? AppMode { get; init; }
    public HttpStatusCode IgUserStatus { get; set; }
    public string? IgUserRaw { get; set; }
    public HttpStatusCode TokenPermsStatus { get; set; }
    public string? TokenPermsRaw { get; set; }
    public HttpStatusCode ProbeStatus { get; set; }
    public string? ProbeRaw { get; set; }
    public override string ToString()
        => $"Mode={AppMode}; IGUser={IgUserStatus}; Token={TokenPermsStatus}; Probe={ProbeStatus}";
}