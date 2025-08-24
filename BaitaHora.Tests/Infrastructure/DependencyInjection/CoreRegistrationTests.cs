using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BaitaHora.Application.Common.Events;
using BaitaHora.Infrastructure.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

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
            // arrange
            var services = new ServiceCollection();
            services.AddLogging();

            // registra um publisher fake só para o teste não explodir
            services.AddSingleton<IPublisher, NoopPublisher>();

            IConfiguration cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            // act
            services.AddInfrastructureCore(cfg); // registra o dispatcher (Scoped) aqui
            var root = services.BuildServiceProvider(validateScopes: true);

            // assert - resolver scoped SEMPRE dentro de um scope
            using var scope = root.CreateScope();
            var dispatcher = scope.ServiceProvider.GetService<IDomainEventDispatcher>();
            Assert.NotNull(dispatcher);
        }
    }
}
