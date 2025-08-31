namespace BaitaHora.Domain.Shared;

public static class IdSet
{
    public static IReadOnlyCollection<Guid> Normalize(IEnumerable<Guid>? ids)
    {
        if (ids is null) return Array.Empty<Guid>();
        var set = new HashSet<Guid>();
        foreach (var id in ids)
            if (id != Guid.Empty) set.Add(id);
        return set.ToArray();
    }

    public static IReadOnlyCollection<Guid> MissingFrom<T>(
        IEnumerable<Guid> requested,
        IEnumerable<T> existing,
        Func<T, Guid> keySelector)
    {
        var req = requested is HashSet<Guid> hs ? hs : new HashSet<Guid>(requested);
        var have = new HashSet<Guid>(existing.Select(keySelector));
        req.ExceptWith(have);
        return req.ToArray();
    }
}