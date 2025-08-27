namespace BaitaHora.Integrations.Social;

public sealed class InstagramOptions
{
    public string InstagramUserId { get; init; } = string.Empty;
    public string AccessToken { get; init; } = string.Empty;
    public string? AppMode { get; init; }
}