namespace BaitaHora.Application.Abstractions.Auth;

public interface ISessionService
{
    Task RegisterLoginAsync(Guid userId, string ip, string userAgent, CancellationToken ct);
}