using BaitaHora.Application.Common.Interfaces;
using BaitaHora.Infrastructure.Data.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class MessagingRegistration
{
    public static IServiceCollection AddMessagingInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<WhatsAppOptions>(config.GetSection("WhatsApp"));

        services.AddScoped<IOutboxPublisher, OutboxPublisher>();

        services.AddHttpClient<WhatsAppBus>();
        services.AddScoped<IBus, WhatsAppBus>();

        return services;
    }
}