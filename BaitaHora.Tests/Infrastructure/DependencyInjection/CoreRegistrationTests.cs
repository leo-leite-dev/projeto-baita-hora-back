using BaitaHora.Application.Common.Events;
using BaitaHora.Infrastructure.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaitaHora.Infrastructure.Tests
{
    public class CoreRegistrationTests
    {
        // Publisher fake para satisfazer a dependência do DomainEventDispatcher
        private sealed class NoopPublisher : IPublisher
        {
            public Task Publish(object notification, CancellationToken cancellationToken = default)
                => Task.CompletedTask;

            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
                where TNotification : INotification
                => Task.CompletedTask;
        }

        [Fact]
        public void AddInfrastructureCore_DeveRegistrar_IDomainEventDispatcher()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddSingleton<IPublisher, NoopPublisher>();

            IConfiguration cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            services.AddInfrastructureCore(cfg); 
            var root = services.BuildServiceProvider(validateScopes: true);

            using var scope = root.CreateScope();
            var dispatcher = scope.ServiceProvider.GetService<IDomainEventDispatcher>();
            Assert.NotNull(dispatcher);
        }
    }
}