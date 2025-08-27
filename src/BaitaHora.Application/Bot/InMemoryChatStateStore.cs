using BaitaHora.Application.Bot;
using System.Collections.Concurrent;

namespace BaitaHora.Infrastructure.Bot;

public sealed class InMemoryChatStateStore : IChatStateStore
{
    private static readonly ConcurrentDictionary<string, IDictionary<string, string>> _store = new();

    public Task<IDictionary<string, string>?> GetAsync(string userId, CancellationToken ct = default)
    {
        _store.TryGetValue(userId, out var state);
        return Task.FromResult(state);
    }

    public Task SetAsync(string userId, IDictionary<string, string> state, CancellationToken ct = default)
    {
        _store[userId] = new Dictionary<string, string>(state);
        return Task.CompletedTask;
    }
}