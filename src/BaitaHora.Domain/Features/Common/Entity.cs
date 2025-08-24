using BaitaHora.Domain.Common.Events;

namespace BaitaHora.Domain.Features.Common;

public abstract class Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity()
    {
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Touch() => UpdatedAtUtc = DateTimeOffset.UtcNow;

    protected void AddDomainEvent(IDomainEvent @event)
        => _domainEvents.Add(@event);

    public void ClearDomainEvents()
        => _domainEvents.Clear();
}