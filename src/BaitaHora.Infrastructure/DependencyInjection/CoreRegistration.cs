using BaitaHora.Application.Common.Errors;
using BaitaHora.Application.IRepositories;
using BaitaHora.Infrastructure.Common.Events;
using BaitaHora.Application.Common.Events;
using BaitaHora.Infrastructure.Data;
using BaitaHora.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BaitaHora.Application.Abstractions.Data;
using BaitaHora.Application.Common.Time;
using BaitaHora.Infrastructure.Common.Time;
using BaitaHora.Infrastructure.Common.Errors;
using BaitaHora.Application.Common.Persistence;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class CoreRegistration
{
    public static IServiceCollection AddInfrastructureCore(this IServiceCollection services, IConfiguration config)
    {

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();
            if (env.IsEnvironment("Testing"))
            {
                // options.UseInMemoryDatabase("BaitaHora_Testing");
            }
            else
            {
                var conn = config.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' não configurada.");
                options.UseNpgsql(conn);
            }
        });

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, EfTransactionalUnitOfWork>();
        services.AddSingleton<IDbErrorTranslator, PostgresDbErrorTranslator>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventAccessor, DomainEventAccessor>();
        services.AddSingleton<IClock, UtcSystemClock>();
        services.AddScoped<IUnitOfWork, EfTransactionalUnitOfWork>();
        services.AddScoped<ITransactionalUnitOfWork, EfTransactionalUnitOfWork>();

        return services;
    }
}