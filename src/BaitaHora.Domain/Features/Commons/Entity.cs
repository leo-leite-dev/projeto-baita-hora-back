namespace BaitaHora.Domain.Features.Commons;

public abstract class Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    protected Entity()
    {
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Touch() => UpdatedAtUtc = DateTimeOffset.UtcNow;
}