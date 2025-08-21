namespace BaitaHora.Application.IServices.Auth;

public interface ISessionService
{
    Task RegisterLoginAsync(Guid userId, string ip, string userAgent, CancellationToken ct);
}