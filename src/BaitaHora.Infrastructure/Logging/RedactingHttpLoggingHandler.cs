// Infrastructure/Http/Logging/RedactingHttpLoggingHandler.cs
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;

public sealed class RedactingHttpLoggingHandler : DelegatingHandler
{
    private readonly ILogger<RedactingHttpLoggingHandler> _logger;
    private readonly string[] _redactKeys = new[] { "access_token", "Authorization" };

    public RedactingHttpLoggingHandler(ILogger<RedactingHttpLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        request.Headers.TryAddWithoutValidation("x-correlation-id", correlationId);

        string Redact(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            foreach (var key in _redactKeys)
            {
                s = System.Text.RegularExpressions.Regex.Replace(
                    s,
                    $@"({key}=)([^&\s]+)",
                    m => $"{m.Groups[1].Value}***REDACTED***",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            return s;
        }

        // Request log
        var reqLine = $"{request.Method} {Redact(request.RequestUri?.ToString() ?? "")}";
        var reqHeaders = string.Join("; ", request.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value)}"));
        string? reqBody = null;
        if (request.Content != null)
        {
            // não consome o stream: bufferiza
            await request.Content.LoadIntoBufferAsync();
            reqBody = await request.Content.ReadAsStringAsync(ct);
        }

        _logger.LogInformation("[IG][HTTP][{CorrelationId}] => {ReqLine}\nHeaders: {Headers}\nBody: {Body}",
            correlationId, reqLine, Redact(reqHeaders), Redact(reqBody ?? "<empty>"));

        // Send
        var response = await base.SendAsync(request, ct);

        // Response log
        var resBody = response.Content != null ? await response.Content.ReadAsStringAsync(ct) : "<empty>";
        _logger.LogInformation("[IG][HTTP][{CorrelationId}] <= {StatusCode}\nBody: {Body}",
            correlationId, (int)response.StatusCode, Redact(resBody));

        // Se Graph retornar erro JSON, loga como erro
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("[IG][ERROR][{CorrelationId}] Graph error {Status}: {Body}",
                correlationId, (int)response.StatusCode, Redact(resBody));
        }

        return response;
    }
}
