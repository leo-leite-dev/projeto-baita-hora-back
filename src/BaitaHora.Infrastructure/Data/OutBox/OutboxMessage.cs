using System.Text.Json;
using BaitaHora.Domain.Common.Events;

namespace BaitaHora.Infrastructure.Data.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;

    public DateTimeOffset OccurredOnUtc { get; set; }
    public DateTimeOffset StoredOnUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? PublishedOnUtc { get; set; }

    public OutboxStatus Status { get; set; } = OutboxStatus.Pending;
    public int AttemptCount { get; set; }
    public DateTimeOffset? NextAttemptUtc { get; set; }
    public string? LastError { get; set; }
    public Guid? LockToken { get; set; }
    public DateTimeOffset? LockedAtUtc { get; set; }

    public static OutboxMessage From(IDomainEvent evt) => new()
    {
        Type = evt.GetType().FullName!,
        Payload = JsonSerializer.Serialize(evt),
        OccurredOnUtc = evt.OccurredOnUtc
    };

    public void MarkPublished()
    {
        PublishedOnUtc = DateTimeOffset.UtcNow;
        Status = OutboxStatus.Published;
        LockToken = null;
        LockedAtUtc = null;
        LastError = null;
    }
}