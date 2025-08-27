using System.Net.Http.Headers;
using BaitaHora.Application.Abstractions.Integrations;
using BaitaHora.Integrations.Social;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options; 

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class BotRegistration
{
    public static IServiceCollection AddBotInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var graphBase = config["META_GRAPH_BASE"]?.TrimEnd('/') ?? "https://graph.facebook.com/v21.0";

        var phoneId = config["META_PHONE_NUMBER_ID"] ?? "";
        var waToken = config["META_PERMANENT_TOKEN"] ?? "";

        services.AddHttpClient<IWhatsAppApi, WhatsAppApiService>((sp, client) =>
        {
            if (string.IsNullOrWhiteSpace(phoneId))
                throw new InvalidOperationException("META_PHONE_NUMBER_ID não definido.");
            if (string.IsNullOrWhiteSpace(waToken))
                throw new InvalidOperationException("META_PERMANENT_TOKEN não definido.");

            client.BaseAddress = new Uri($"{graphBase}/{phoneId}/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", waToken);
        });

        services.AddHttpClient<IInstagramApi, InstagramApi>((sp, client) =>
        {
            var opt = sp.GetRequiredService<IOptions<InstagramOptions>>().Value;

            client.BaseAddress = new Uri($"{graphBase}/");
            if (!string.IsNullOrWhiteSpace(opt.AccessToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", opt.AccessToken);
            }
        });

        return services;
    }
}