using System.Net.Http.Json;

namespace BaitaHora.Seeder.Http
{
    public sealed class ApiClient : IDisposable
    {
        private readonly HttpClient _http;

        public ApiClient(string baseUrl)
        {
            _http = new HttpClient { BaseAddress = new Uri(baseUrl, UriKind.Absolute) };
        }

        public void SetBearer(string token)
            => _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        public void ClearBearer()
            => _http.DefaultRequestHeaders.Authorization = null;

        public void Dispose() => _http.Dispose();

        public async Task<(int status, string? body)> LoginAsync(string identify, string password, CancellationToken ct)
            => await PostAndReadAsync(_http.BaseAddress!.ToString(), "/api/auth/login", new { identify, password }, ct);

        public async Task<(int status, string? body)> PostAndReadAsync(string baseUrl, string path, object payload, CancellationToken ct)
            => await SendAndReadAsync(HttpMethod.Post, baseUrl, path, payload, ct);

        public async Task<(int status, string? body)> PutAndReadAsync(string baseUrl, string path, object payload, CancellationToken ct)
            => await SendAndReadAsync(HttpMethod.Put, baseUrl, path, payload, ct);

        public async Task<(int status, string? body)> PatchAndReadAsync(string baseUrl, string path, object payload, CancellationToken ct)
            => await SendAndReadAsync(HttpMethod.Patch, baseUrl, path, payload, ct);

        public async Task<(int status, string? body)> DeleteAndReadAsync(string baseUrl, string path, CancellationToken ct)
        {
            EnsureBase(baseUrl);
            using var req = new HttpRequestMessage(HttpMethod.Delete, path);
            using var res = await _http.SendAsync(req, ct);
            var body = await res.Content.ReadAsStringAsync(ct);
            return ((int)res.StatusCode, body);
        }

        private async Task<(int status, string? body)> SendAndReadAsync(HttpMethod method, string baseUrl, string path, object payload, CancellationToken ct)
        {
            EnsureBase(baseUrl);
            using var req = new HttpRequestMessage(method, path)
            {
                Content = JsonContent.Create(payload)
            };
            using var res = await _http.SendAsync(req, ct);
            var body = await res.Content.ReadAsStringAsync(ct);
            return ((int)res.StatusCode, body);
        }

        private void EnsureBase(string baseUrl)
        {
            if (_http.BaseAddress?.ToString() != baseUrl)
                _http.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
        }
    }
}
