using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Events;
using BaitaHora.Application.Common.Time;
using BaitaHora.Application.Features.Schedules;
using BaitaHora.Application.Features.Users.Handlers;
using BaitaHora.Domain.Common.Events;
using BaitaHora.Domain.Features.Schedules;
using BaitaHora.Domain.Features.Users.Events;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Application.Features.Schedulings.Handlers;

// Comando fake só para ativar o pipeline
public sealed record PipelineFakeCommand() : IRequest<string>;

// Handler fake do comando (o objetivo é só passar pelo pipeline)
public sealed class PipelineFakeCommandHandler : IRequestHandler<PipelineFakeCommand, string>
{
    public Task<string> Handle(PipelineFakeCommand request, CancellationToken ct)
        => Task.FromResult("ok");
}

public class CreateSchedule_PipelineSmokeTests
{
    [Fact]
    public async Task Pipeline_Should_Publish_UserRegistered_And_Invoke_Handler()
    {
        // arrange: DI com MediatR + Behavior real
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMediatR(cfg =>
        {
            // Handlers reais (inclui CreateScheduleWhenUserRegisteredHandler)
            cfg.RegisterServicesFromAssemblyContaining<CreateScheduleWhenUserRegisteredHandler>();
            // Também registra o assembly onde está o handler fake do comando
            cfg.RegisterServicesFromAssemblyContaining<PipelineFakeCommandHandler>();
        });

        // ✅ registra o Behavior real no pipeline
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventsBehavior<,>));

        // 🔧 mocks necessários
        var accessor = new Mock<IDomainEventAccessor>();
        var repo     = new Mock<IScheduleRepository>();
        var clock    = new Mock<IClock>();

        var userId = Guid.NewGuid();
        var domainEvent = new UserRegisteredDomainEvent(userId);

        // Quando o Behavior coletar eventos, devolve nosso UserRegistered
        accessor.Setup(a => a.CollectDomainEventsAndClear())
                .Returns(new List<IDomainEvent> { domainEvent });

        // O handler não deve ver duplicidade
        repo.Setup(r => r.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Schedule? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<Schedule>(), It.IsAny<CancellationToken>()))
            .Callback<Schedule, CancellationToken>((s, _) => captured = s)
            .Returns(Task.CompletedTask);

        var fixedNow = new DateTime(2025, 08, 24, 10, 00, 00, DateTimeKind.Utc);
        clock.SetupGet(c => c.UtcNow).Returns(fixedNow);

        // Injeta mocks
        services.AddSingleton(accessor.Object);
        services.AddSingleton(repo.Object);
        services.AddSingleton(clock.Object);

        // ✅ Dispatcher que publica DomainEventNotification<T> via IMediator
        services.AddSingleton<IDomainEventDispatcher>(sp =>
            new TestMediatorBackedDispatcher(sp.GetRequiredService<IMediator>()));

        // ✅ registra explicitamente o handler do comando fake
        services.AddTransient<IRequestHandler<PipelineFakeCommand, string>, PipelineFakeCommandHandler>();

        var sp = services.BuildServiceProvider();
        var mediator = sp.GetRequiredService<IMediator>();

        // act: ao enviar o comando, o Behavior coleta o evento
        // e o dispatcher publica a notificação, disparando o handler real
        var result = await mediator.Send(new PipelineFakeCommand());

        // assert
        result.Should().Be("ok");
        captured.Should().NotBeNull("o handler do evento deve criar o Schedule");
        captured!.UserId.Should().Be(userId);

        accessor.Verify(a => a.CollectDomainEventsAndClear(), Times.Once);
        repo.Verify(r => r.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<Schedule>(), It.IsAny<CancellationToken>()), Times.Once);
        clock.VerifyGet(c => c.UtcNow, Times.Once);
        repo.VerifyNoOtherCalls();
    }

    // Dispatcher simples que converte IEnumerable<IDomainEvent> em DomainEventNotification<T> e publica via IMediator
    private sealed class TestMediatorBackedDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;
        public TestMediatorBackedDispatcher(IMediator mediator) => _mediator = mediator;

        public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct)
        {
            var tasks = new List<Task>();
            foreach (var ev in domainEvents)
            {
                var evType   = ev.GetType();
                var notifTyp = typeof(DomainEventNotification<>).MakeGenericType(evType);
                var notif    = Activator.CreateInstance(notifTyp, ev)!;
                tasks.Add(_mediator.Publish((INotification)notif, ct));
            }
            return Task.WhenAll(tasks);
        }
    }
}
