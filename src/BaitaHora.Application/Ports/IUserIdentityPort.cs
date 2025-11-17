namespace BaitaHora.Application.Ports;

public interface IUserIdentityPort
{
    Guid GetMemberId();

    Task<(string Username, IEnumerable<string> Roles, bool IsActive)>
        GetIdentityAsync(Guid memberId, CancellationToken ct);
}