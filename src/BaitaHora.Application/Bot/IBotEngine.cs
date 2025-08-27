namespace BaitaHora.Application.Bot;

public interface IBotEngine
{
    Task<BotReply> HandleAsync(BotContext ctx, CancellationToken ct);
}

public sealed record BotContext(
    string UserId,
    string MessageText,
    IDictionary<string, string> Session
);

public sealed record BotReply(
    string Text,
    IDictionary<string, string>? NewSession = null
);