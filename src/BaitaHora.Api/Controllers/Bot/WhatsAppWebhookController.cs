// Api/Controllers/Webhooks/WhatsappWebhookController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using BaitaHora.Application.Bot; // IBotEngine
using BaitaHora.Application.Abstractions.Integrations; // IWhatsAppApi
using BaitaHora.Application.Bot;
using BaitaHora.Integrations.Social; // BotContext

[ApiController]
[Route("webhooks/whatsapp")]
public sealed class WhatsappWebhookController : ControllerBase
{
    private readonly IBotEngine _bot;
    private readonly IWhatsAppApi _wa;
    private readonly IChatStateStore _state; // tua store em memória
    private readonly string _verifyToken;
    private readonly string _appSecret;

    public WhatsappWebhookController(
        IBotEngine bot, IWhatsAppApi wa, IChatStateStore state,
        IOptions<MetaOptions> opts)
    {
        _bot = bot; _wa = wa; _state = state;
        _verifyToken = opts.Value.VerifyToken;
        _appSecret = opts.Value.AppSecret;
    }

    // GET /webhooks/whatsapp?hub.mode=subscribe&hub.challenge=...&hub.verify_token=...
    [HttpGet]
    public IActionResult Verify([FromQuery] string? hub_mode, [FromQuery] string? hub_challenge, [FromQuery] string? hub_verify_token)
        => (hub_mode == "subscribe" && hub_verify_token == _verifyToken)
           ? Content(hub_challenge ?? string.Empty)
           : Unauthorized();

    [HttpPost]
    public async Task<IActionResult> Receive()
    {
        // 1) validar assinatura
        var signature = Request.Headers["X-Hub-Signature-256"].ToString();
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        if (!ValidateSignature(_appSecret, body, signature)) return Unauthorized();

        // 2) parse mínimo (mensagem de texto)
        var json = System.Text.Json.JsonDocument.Parse(body);
        var changes = json.RootElement.GetProperty("entry")[0].GetProperty("changes")[0].GetProperty("value");
        var messages = changes.TryGetProperty("messages", out var msgs) ? msgs : default;
        if (messages.ValueKind != System.Text.Json.JsonValueKind.Array) return Ok();

        foreach (var m in messages.EnumerateArray())
        {
            var from = m.GetProperty("from").GetString()!;
            var type = m.GetProperty("type").GetString();
            if (type != "text") continue;
            var text = m.GetProperty("text").GetProperty("body").GetString() ?? string.Empty;

            // 3) carrega estado, chama engine e envia resposta
            var session = await _state.GetAsync(from) ?? new Dictionary<string,string>();
            var reply = await _bot.HandleAsync(new BotContext(from, text, session), HttpContext.RequestAborted); // :contentReference[oaicite:5]{index=5}
            if (reply.NewSession is not null) await _state.SetAsync(from, reply.NewSession); // 

            await _wa.SendTextAsync(from, reply.Text); // envia pela API do WhatsApp :contentReference[oaicite:7]{index=7}
        }
        return Ok();
    }

    private static bool ValidateSignature(string appSecret, string payload, string header)
    {
        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("sha256=")) return false;
        var sig = header["sha256=".Length..];
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var hex = Convert.ToHexString(hash).ToLowerInvariant();
        return CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(sig), Encoding.ASCII.GetBytes(hex));
    }
}
