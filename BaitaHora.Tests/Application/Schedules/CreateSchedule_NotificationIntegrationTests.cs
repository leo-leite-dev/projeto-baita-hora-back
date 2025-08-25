using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MediatR;
using BaitaHora.Application.Common.Events;
using BaitaHora.Application.Common.Time;
using BaitaHora.Application.Features.Schedules;
using BaitaHora.Application.Features.Users.Handlers;
using BaitaHora.Domain.Features.Users.Events;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Application.Features.Schedulings.Handlers;

public class CreateSchedule_NotificationIntegrationTests
{
    [Fact]
    public async Task Mediator_Should_Invoke_Handler_On_DomainEventNotification_UserRegistered()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateScheduleWhenUserRegisteredHandler>();
        });

        var repo = new Mock<IScheduleRepository>();
        var clock = new Mock<IClock>();

        var userId = Guid.NewGuid();
        repo.Setup(r => r.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var fixedNow = new DateTime(2025, 08, 24, 10, 00, 00, DateTimeKind.Utc);
        clock.SetupGet(c => c.UtcNow).Returns(fixedNow);

        Schedule? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<Schedule>(), It.IsAny<CancellationToken>()))
            .Callback<Schedule, CancellationToken>((s, _) => captured = s)
            .Returns(Task.CompletedTask);

        services.AddSingleton(repo.Object);
        services.AddSingleton(clock.Object);

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        var notification = new DomainEventNotification<UserRegisteredDomainEvent>(
            new UserRegisteredDomainEvent(userId)
        );

        // act
        await mediator.Publish(notification);

        // assert
        captured.Should().NotBeNull();
        captured!.UserId.Should().Be(userId);

        repo.Verify(r => r.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<Schedule>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
        clock.VerifyGet(c => c.UtcNow, Times.Once);
    }
}
