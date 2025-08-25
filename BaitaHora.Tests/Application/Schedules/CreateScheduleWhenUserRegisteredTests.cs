using BaitaHora.Application.Common.Events;
using BaitaHora.Application.Common.Time;
using BaitaHora.Application.Features.Schedules;
using BaitaHora.Domain.Features.Users.Events;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Application.Features.Schedulings.Handlers;
using FluentAssertions;
using Moq;

public class CreateScheduleWhenUserRegisteredTests
{
    [Fact]
    public async Task Handler_Should_Create_Schedule_When_UserRegistered_And_Not_Exists()
    {
        // arrange
        var userId = Guid.NewGuid();
        var evt = new UserRegisteredDomainEvent(userId);
        var notification = new DomainEventNotification<UserRegisteredDomainEvent>(evt);

        var repo = new Mock<IScheduleRepository>();
        repo.Setup(r => r.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Schedule? captured = null;
        repo.Setup(r => r.AddAsync(It.IsAny<Schedule>(), It.IsAny<CancellationToken>()))
            .Callback<Schedule, CancellationToken>((s, _) => captured = s)
            .Returns(Task.CompletedTask);

        var fixedNow = new DateTime(2025, 08, 24, 10, 00, 00, DateTimeKind.Utc);
        var clock = new Mock<IClock>();
        clock.SetupGet(c => c.UtcNow).Returns(fixedNow);

        var handler = new CreateScheduleWhenUserRegisteredHandler(repo.Object, clock.Object);

        // act
        await handler.Handle(notification, CancellationToken.None);

        // assert
        captured.Should().NotBeNull("o handler deve criar um Schedule");
        captured!.UserId.Should().Be(userId);

        repo.Verify(r => r.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<Schedule>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();

        clock.VerifyGet(c => c.UtcNow, Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Not_Create_Schedule_When_One_Already_Exists()
    {
        // arrange
        var userId = Guid.NewGuid();
        var evt = new UserRegisteredDomainEvent(userId);
        var notification = new DomainEventNotification<UserRegisteredDomainEvent>(evt);

        var repo = new Mock<IScheduleRepository>();
        repo.Setup(r => r.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // << já existe

        var clock = new Mock<IClock>();

        var handler = new CreateScheduleWhenUserRegisteredHandler(repo.Object, clock.Object);

        // act
        await handler.Handle(notification, CancellationToken.None);

        // assert
        repo.Verify(r => r.ExistsForUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.AddAsync(It.IsAny<Schedule>(), It.IsAny<CancellationToken>()), Times.Never);
        clock.VerifyGet(c => c.UtcNow, Times.Never); // não deveria nem consultar o relógio
        repo.VerifyNoOtherCalls();
    }
}