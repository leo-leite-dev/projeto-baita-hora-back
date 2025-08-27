namespace BaitaHora.Application.Bot;

public sealed class SimpleBotEngine : IBotEngine
{
    public Task<BotReply> HandleAsync(BotContext ctx, CancellationToken ct)
    {
        var session = new Dictionary<string, string>(ctx.Session);

        if (!session.ContainsKey("registered"))
        {
            session["registered"] = "yes";
            return Task.FromResult(new BotReply(
                "👋 Olá! Você foi cadastrado no BaitaHora ✅\nDigite 'ajuda' para ver opções.",
                session));
        }

        var msg = ctx.MessageText.Trim().ToLowerInvariant();

        if (msg.Contains("ajuda"))
            return Task.FromResult(new BotReply("📋 Opções: 'agendar', 'cancelar', 'ajuda'"));

        if (msg.Contains("agendar"))
        {
            session["flow"] = "scheduling";
            return Task.FromResult(new BotReply("Qual serviço você deseja agendar?", session));
        }

        if (session.TryGetValue("flow", out var flow) && flow == "scheduling")
        {
            session.Remove("flow");
            return Task.FromResult(new BotReply($"Serviço '{ctx.MessageText}' anotado ✅", session));
        }

        return Task.FromResult(new BotReply("❓ Não entendi. Digite 'ajuda' para opções."));
    }
}