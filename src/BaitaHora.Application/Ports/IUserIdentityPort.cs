namespace BaitaHora.Application.Ports;

public interface IUserIdentityPort
{
    Guid GetUserId();

    Task<(string Username, IEnumerable<string> Roles, bool IsActive)>
        GetIdentityAsync(Guid userId, CancellationToken ct);
}