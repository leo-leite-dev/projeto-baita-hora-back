using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BaitaHora.Application.Abstractions.Integrations;
using BaitaHora.Integrations.Social;

[ApiController]
[Route("webhooks/instagram")]
public sealed class InstagramWebhookController : ControllerBase
{
    private readonly IInstagramApi _ig;
    private readonly MetaOptions _meta;

    public InstagramWebhookController(IInstagramApi ig, IOptions<MetaOptions> meta)
    {
        _ig = ig;
        _meta = meta.Value;
    }

    // GET /webhooks/instagram?hub.mode=subscribe&hub.verify_token=...&hub.challenge=...
    [HttpGet]
    public IActionResult Verify(
        [FromQuery(Name = "hub.mode")] string? mode,
        [FromQuery(Name = "hub.verify_token")] string? token,
        [FromQuery(Name = "hub.challenge")] string? challenge)
    {
        var ok = mode == "subscribe" && token == _meta.VerifyToken;
        Console.WriteLine($"[WEBHOOK][VERIFY] mode={mode} tokenMatch={ok}");
        return ok ? Content(challenge ?? string.Empty, "text/plain") : Unauthorized();
    }

    [HttpPost]
    public async Task<IActionResult> Receive()
    {
        // lê o RAW body (necessário para validar a assinatura)
        using var ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms);
        var raw = ms.ToArray();

        if (_meta.ValidateSignature && !IsValidSignature(Request, raw, _meta.AppSecret))
        {
            Console.WriteLine("[WEBHOOK] assinatura inválida (X-Hub-Signature-256).");
            return Ok(); // sempre 200 pra Meta não reentregar em loop
        }

        var body = JsonDocument.Parse(raw).RootElement;
        Console.WriteLine("[WEBHOOK][RECEIVE] payload: " + body.ToString());

        // Ignora payload de teste do painel “Enviar para servidor”
        if (IsPanelTest(body))
        {
            Console.WriteLine("[WEBHOOK] payload de TESTE do painel (ignorado).");
            return Ok();
        }

        if (!body.TryGetProperty("entry", out var entries) || entries.ValueKind != JsonValueKind.Array)
            return Ok();

        foreach (var entry in entries.EnumerateArray())
        {
            var igBusinessId = entry.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
            if (string.IsNullOrWhiteSpace(igBusinessId))
            {
                Console.WriteLine("[WEBHOOK] entry.id ausente (IG Business ID).");
                continue;
            }

            // -------- IG formato “changes” (principal para Instagram) --------
            if (entry.TryGetProperty("changes", out var changes) && changes.ValueKind == JsonValueKind.Array)
            {
                foreach (var change in changes.EnumerateArray())
                {
                    var field = change.TryGetProperty("field", out var fieldProp) ? fieldProp.GetString() : null;
                    if (!change.TryGetProperty("value", out var value)) continue;

                    var senderId = TryGetChangeSenderId(value, out var sid) ? sid : string.Empty;

                    if (field == "messages")
                    {
                        // message.text
                        if (value.TryGetProperty("message", out var vMsg) &&
                            vMsg.TryGetProperty("text", out var vText))
                        {
                            var text = vText.GetString() ?? string.Empty;
                            Console.WriteLine($"[IG][changes] de {senderId}: {text}");

                            if (!string.IsNullOrWhiteSpace(senderId))
                                await _ig.SendTextAsync(igBusinessId!, senderId, $"Recebi: {text}", HttpContext.RequestAborted);
                        }
                        else
                        {
                            Console.WriteLine($"[IG][changes] messages sem text: {value}");
                        }
                        continue;
                    }

                    if (field == "message_reads")
                    {
                        if (value.TryGetProperty("read", out var vRead) &&
                            vRead.TryGetProperty("watermark", out var wm))
                        {
                            Console.WriteLine($"[IG][changes] read de {senderId}: watermark={wm.GetString()}");
                        }
                        continue;
                    }

                    Console.WriteLine($"[IG][changes] field não tratado: {field} → {value}");
                }
            }

            // -------- Messenger payload (não atrapalha manter) --------
            if (entry.TryGetProperty("messaging", out var messaging) && messaging.ValueKind == JsonValueKind.Array)
            {
                foreach (var evt in messaging.EnumerateArray())
                {
                    if (!TryGetSenderId(evt, out var senderId)) continue;

                    // texto
                    if (evt.TryGetProperty("message", out var msg) &&
                        msg.TryGetProperty("text", out var textProp))
                    {
                        var text = textProp.GetString() ?? string.Empty;
                        Console.WriteLine($"[MSGR] de {senderId}: {text}");
                        await _ig.SendTextAsync(igBusinessId!, senderId, $"Recebi: {text}", HttpContext.RequestAborted);
                        continue;
                    }

                    // read
                    if (evt.TryGetProperty("read", out var readObj) &&
                        readObj.TryGetProperty("watermark", out var wmProp))
                    {
                        Console.WriteLine($"[MSGR] read de {senderId}: watermark={wmProp.GetString()}");
                        continue;
                    }

                    // postback
                    if (evt.TryGetProperty("postback", out var postback) &&
                        postback.TryGetProperty("payload", out var payloadProp))
                    {
                        var payload = payloadProp.GetString() ?? string.Empty;
                        Console.WriteLine($"[MSGR] postback de {senderId}: {payload}");
                        await _ig.SendTextAsync(igBusinessId!, senderId, $"Recebi teu postback: {payload}", HttpContext.RequestAborted);
                        continue;
                    }

                    Console.WriteLine($"[MSGR] evento não tratado de {senderId}: {evt}");
                }
            }
        }

        return Ok();
    }

    // ------------ helpers ------------

    private static bool TryGetSenderId(JsonElement evt, out string senderId)
    {
        senderId = string.Empty;
        if (evt.TryGetProperty("sender", out var sender) &&
            sender.TryGetProperty("id", out var sidProp))
        {
            senderId = sidProp.GetString() ?? string.Empty;
        }
        return !string.IsNullOrWhiteSpace(senderId);
    }

    private static bool TryGetChangeSenderId(JsonElement value, out string senderId)
    {
        senderId = string.Empty;
        if (value.TryGetProperty("sender", out var vSender) &&
            vSender.TryGetProperty("id", out var vSid))
        {
            senderId = vSid.GetString() ?? string.Empty;
        }
        return !string.IsNullOrWhiteSpace(senderId);
    }

    private static bool IsPanelTest(JsonElement body)
    {
        try
        {
            if (!body.TryGetProperty("entry", out var entries) || entries.ValueKind != JsonValueKind.Array)
                return false;

            var entry0 = entries[0];

            // id "0" é comum no teste do painel
            if (entry0.TryGetProperty("id", out var id0) && id0.GetString() == "0")
                return true;

            // changes[].value.message.* com "random_*"
            if (entry0.TryGetProperty("changes", out var changes) && changes.ValueKind == JsonValueKind.Array)
            {
                var value = changes[0].GetProperty("value");
                if (value.TryGetProperty("message", out var msg))
                {
                    if (msg.TryGetProperty("mid", out var mid) && (mid.GetString() ?? "").Contains("random_"))
                        return true;
                    if (msg.TryGetProperty("text", out var txt) && (txt.GetString() ?? "").Contains("random_"))
                        return true;
                }
            }

            // messaging[].message.* com "random_*"
            if (entry0.TryGetProperty("messaging", out var messaging) && messaging.ValueKind == JsonValueKind.Array)
            {
                var ev = messaging[0];
                if (ev.TryGetProperty("message", out var m))
                {
                    if (m.TryGetProperty("mid", out var mid) && (mid.GetString() ?? "").Contains("random_"))
                        return true;
                    if (m.TryGetProperty("text", out var txt) && (txt.GetString() ?? "").Contains("random_"))
                        return true;
                }
            }
        }
        catch { /* ignore */ }

        return false;
    }

    private static bool IsValidSignature(HttpRequest req, byte[] rawBody, string appSecret)
    {
        if (string.IsNullOrWhiteSpace(appSecret)) return false;
        if (!req.Headers.TryGetValue("X-Hub-Signature-256", out var sigHeader)) return false;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
        var expected = "sha256=" + Convert.ToHexString(hmac.ComputeHash(rawBody)).ToLowerInvariant();
        return string.Equals(sigHeader.ToString(), expected, StringComparison.OrdinalIgnoreCase);
    }
}