namespace BaitaHora.Application.Common.Interfaces;

public interface IOutboxPublisher
{
    Task PublishPendingAsync(CancellationToken ct);
}