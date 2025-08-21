using System.Collections.Concurrent;
using BaitaHora.Application.Ports;

namespace BaitaHora.Infrastructure.Adapters.Auth;

public sealed class CachingUserIdentityAdapter : IUserIdentityPort
{
    private readonly IUserIdentityPort _inner;
    private readonly TimeSpan _ttl;
    private readonly ConcurrentDictionary<Guid, (DateTime When, (string U, IEnumerable<string> R, bool A) Data)> _cache = new();

    public CachingUserIdentityAdapter(IUserIdentityPort inner, TimeSpan? ttl = null)
    {
        _inner = inner;
        _ttl = ttl ?? TimeSpan.FromSeconds(60);
    }

    public async Task<(string Username, IEnumerable<string> Roles, bool IsActive)> GetIdentityAsync(Guid userId, CancellationToken ct)
    {
        if (_cache.TryGetValue(userId, out var entry) && (DateTime.UtcNow - entry.When) < _ttl)
            return entry.Data;

        var data = await _inner.GetIdentityAsync(userId, ct);
        _cache[userId] = (DateTime.UtcNow, (data.Username, data.Roles.ToArray(), data.IsActive));
        return data;
    }
}