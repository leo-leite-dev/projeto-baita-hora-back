using System.Security.Claims;
using BaitaHora.Application.Ports;
using Microsoft.AspNetCore.Http;

namespace BaitaHora.Infrastructure.Services.Auth
{
    public sealed class HttpContextUserIdentityAdapter : IUserIdentityPort
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextUserIdentityAdapter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<(string Username, IEnumerable<string> Roles, bool IsActive)> GetIdentityAsync(Guid userId, CancellationToken ct)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var username = user?.Identity?.Name ?? "unknown";
            var roles = user?.FindAll(ClaimTypes.Role).Select(r => r.Value) ?? Enumerable.Empty<string>();
            var isActive = true;

            return Task.FromResult((username, roles, isActive));
        }
    }
}