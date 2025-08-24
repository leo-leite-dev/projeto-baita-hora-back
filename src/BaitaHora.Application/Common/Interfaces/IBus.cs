namespace BaitaHora.Application.Common.Interfaces;

public interface IBus
{
    Task PublishAsync(string topic, string payloadJson, IDictionary<string, string>? headers = null, CancellationToken ct = default);
}