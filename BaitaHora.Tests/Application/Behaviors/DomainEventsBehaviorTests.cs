using FluentAssertions;
using MediatR;
using Moq;
using BaitaHora.Application.Common.Behaviors;
using BaitaHora.Application.Common.Events;
using BaitaHora.Domain.Common.Events;
using BaitaHora.Domain.Features.Users.Events;

// Request fake só para passar pelo pipeline
public sealed record FakeCommand() : IRequest<string>;

public class DomainEventsBehaviorTests
{
    [Fact]
    public async Task Should_Dispatch_DomainEvents_When_Collected()
    {
        // arrange
        var accessor = new Mock<IDomainEventAccessor>();
        var dispatcher = new Mock<IDomainEventDispatcher>();

        var ev = new UserRegisteredDomainEvent(Guid.NewGuid());
        var collected = new List<IDomainEvent> { ev };

        accessor.Setup(a => a.CollectDomainEventsAndClear())
                .Returns(collected);

        // Em teu build, o delegate recebe CancellationToken
        RequestHandlerDelegate<string> next = (ct) => Task.FromResult("ok");

        var behavior = new DomainEventsBehavior<FakeCommand, string>(accessor.Object, dispatcher.Object);

        // act
        var response = await behavior.Handle(new FakeCommand(), next, CancellationToken.None);

        // assert
        response.Should().Be("ok");
        accessor.Verify(a => a.CollectDomainEventsAndClear(), Times.Once);

        dispatcher.Verify(d => d.DispatchAsync(
            It.Is<IEnumerable<IDomainEvent>>(list => ReferenceEquals(list, collected)),
            It.IsAny<CancellationToken>()),
            Times.Once);

        dispatcher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Should_Not_Dispatch_When_No_DomainEvents()
    {
        // arrange
        var accessor = new Mock<IDomainEventAccessor>();
        var dispatcher = new Mock<IDomainEventDispatcher>();

        accessor.Setup(a => a.CollectDomainEventsAndClear())
                .Returns(new List<IDomainEvent>()); // vazio

        RequestHandlerDelegate<string> next = (ct) => Task.FromResult("ok");

        var behavior = new DomainEventsBehavior<FakeCommand, string>(accessor.Object, dispatcher.Object);

        // act
        var response = await behavior.Handle(new FakeCommand(), next, CancellationToken.None);

        // assert
        response.Should().Be("ok");
        accessor.Verify(a => a.CollectDomainEventsAndClear(), Times.Once);
        dispatcher.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()), Times.Never);
        dispatcher.VerifyNoOtherCalls();
    }
}
