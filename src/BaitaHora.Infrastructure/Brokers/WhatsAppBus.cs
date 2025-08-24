using BaitaHora.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class WhatsAppOptions
{
    public string BaseUrl { get; set; } = default!;
    public string Token { get; set; } = default!;  
}

public sealed class WhatsAppBus : IBus
{
    private readonly HttpClient _http;
    private readonly WhatsAppOptions _opt;
    private readonly ILogger<WhatsAppBus> _logger;

    public WhatsAppBus(HttpClient http, IOptions<WhatsAppOptions> opt, ILogger<WhatsAppBus> logger)
    {
        _http = http;
        _opt = opt.Value;
        _logger = logger;
    }

    public async Task PublishAsync(string topic, string payloadJson,
        IDictionary<string, string>? headers = null, CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_opt.BaseUrl}/messages");
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _opt.Token);
        req.Content = new StringContent(payloadJson, System.Text.Encoding.UTF8, "application/json");

        var res = await _http.SendAsync(req, ct);
        if (!res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"WhatsApp API falhou ({(int)res.StatusCode}): {body}");
        }

        _logger.LogDebug("WhatsApp enviado (topic={Topic})", topic);
    }
}