namespace BaitaHora.Application.Ports;

public interface IUserIdentityPort
{
    Task<(string Username, IEnumerable<string> Roles, bool IsActive)> GetIdentityAsync(
        Guid userId, CancellationToken ct);
}