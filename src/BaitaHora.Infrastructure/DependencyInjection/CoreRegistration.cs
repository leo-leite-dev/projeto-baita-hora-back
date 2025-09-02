using BaitaHora.Application.Abstractions.Data;
using BaitaHora.Application.Common.Errors;
using BaitaHora.Application.Common.Events;
using BaitaHora.Application.Common.Persistence;
using BaitaHora.Application.Common.Time;
using BaitaHora.Application.IRepositories;
using BaitaHora.Infrastructure.Common.Errors;
using BaitaHora.Infrastructure.Common.Events;
using BaitaHora.Infrastructure.Common.Time;
using BaitaHora.Infrastructure.Data;
using BaitaHora.Infrastructure.Data.Behaviors;
using BaitaHora.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BaitaHora.Infrastructure.DependencyInjection;

public static class CoreRegistration
{
    public static IServiceCollection AddInfrastructureCore(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();

            var conn = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' não configurada.");
            options.UseNpgsql(conn);

            if (env.IsDevelopment() || env.IsEnvironment("Local"))
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        });

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddSingleton<IDbErrorTranslator, PostgresDbErrorTranslator>();

        services.AddScoped<IUnitOfWork, EfTransactionalUnitOfWork>();
        services.AddScoped<ITransactionalUnitOfWork, EfTransactionalUnitOfWork>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventAccessor, DomainEventAccessor>();

        services.AddSingleton<IClock, UtcSystemClock>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfPersistenceExceptionMappingBehavior<,>));

        return services;
    }
}