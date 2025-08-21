namespace BaitaHora.Domain.Commons;

public abstract class Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    protected Entity()
    {
        var now = DateTimeOffset.UtcNow;
        CreatedAtUtc = now;
        UpdatedAtUtc = now;
    }

    public void Touch() => UpdatedAtUtc = DateTimeOffset.UtcNow;
}