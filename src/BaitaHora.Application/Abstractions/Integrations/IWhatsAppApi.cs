namespace BaitaHora.Application.Abstractions.Integrations;

public interface IWhatsAppApi
{
    Task SendTextAsync(string to, string text, CancellationToken ct = default);
}