using System.Net.Http.Json;
using BaitaHora.Application.Abstractions.Integrations;

namespace BaitaHora.Integrations.Social;

public sealed class WhatsAppApiService(HttpClient http) : IWhatsAppApi
{
    private readonly HttpClient _http = http;

    public async Task SendTextAsync(string to, string text, CancellationToken ct = default)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            to,
            type = "text",
            text = new { body = text }
        };

        var resp = await _http.PostAsJsonAsync("messages", payload, ct);
        resp.EnsureSuccessStatusCode();
    }
}