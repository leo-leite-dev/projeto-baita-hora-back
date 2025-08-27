namespace BaitaHora.Application.Bot;

public interface IChatStateStore
{
    Task<IDictionary<string,string>?> GetAsync(string userId, CancellationToken ct = default);
    Task SetAsync(string userId, IDictionary<string,string> state, CancellationToken ct = default);
}
