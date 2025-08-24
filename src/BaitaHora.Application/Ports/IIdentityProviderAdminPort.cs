namespace BaitaHora.Application.Ports;

public interface IIdentityProviderAdminPort
{
    Task InvalidateUserSessionsAsync(Guid userId, CancellationToken ct);
}