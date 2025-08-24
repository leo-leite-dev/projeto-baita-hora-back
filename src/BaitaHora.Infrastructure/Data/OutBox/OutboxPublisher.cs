using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BaitaHora.Application.Common.Interfaces;

namespace BaitaHora.Infrastructure.Data.Outbox;

public sealed class OutboxPublisher : IOutboxPublisher
{
    private readonly DbContext _db;
    private readonly IBus _bus;
    private readonly ILogger<OutboxPublisher> _logger;

    private const int BatchSize = 200;
    private static readonly TimeSpan LockStaleAfter = TimeSpan.FromMinutes(2);
    private const int MaxAttempts = 10;

    public OutboxPublisher(DbContext db, IBus bus, ILogger<OutboxPublisher> logger)
    {
        _db = db;
        _bus = bus;
        _logger = logger;
    }

    public async Task PublishPendingAsync(CancellationToken ct)
    {
        var workerId = Guid.NewGuid();

        var items = await ClaimBatchAsync(workerId, ct);
        if (items.Count == 0) return;

        foreach (var msg in items)
        {
            try
            {
                IDictionary<string, string>? headers = null;

                await _bus.PublishAsync(
                    topic: msg.Type,
                    payloadJson: msg.Payload,
                    headers: headers,
                    ct: ct);

                msg.MarkPublished();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao publicar Outbox {MessageId} (type {Type}), tentativa {Attempt}",
                    msg.Id, msg.Type, msg.AttemptCount + 1);

                msg.AttemptCount += 1;
                msg.LastError = ex.Message;

                if (msg.AttemptCount >= MaxAttempts)
                {
                    msg.Status = OutboxStatus.Failed;
                    msg.NextAttemptUtc = null;
                }
                else
                {
                    msg.Status = OutboxStatus.Pending;
                    msg.NextAttemptUtc = ComputeBackoffUtc(msg.AttemptCount);
                }

                msg.LockToken = null;
                msg.LockedAtUtc = null;
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    private static DateTimeOffset ComputeBackoffUtc(int attempt)
    {
        var seconds = Math.Min(Math.Pow(2, attempt), 300);
        return DateTimeOffset.UtcNow.AddSeconds(seconds);
    }

    private async Task<List<OutboxMessage>> ClaimBatchAsync(Guid workerId, CancellationToken ct)
    {
        var lockStaleThreshold = DateTimeOffset.UtcNow - LockStaleAfter;

        var eligible = await _db.Set<OutboxMessage>()
            .FromSqlRaw(@"
                SELECT *
                FROM outbox_messages
                WHERE ""Status"" = {0}
                  AND (""NextAttemptUtc"" IS NULL OR ""NextAttemptUtc"" <= NOW())
                  AND (""LockedAtUtc"" IS NULL OR ""LockedAtUtc"" <= {1})
                ORDER BY ""StoredOnUtc""
                FOR UPDATE SKIP LOCKED
                LIMIT {2}
            ", (short)OutboxStatus.Pending, lockStaleThreshold, BatchSize)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (eligible.Count == 0)
            return new();

        var now = DateTimeOffset.UtcNow;
        await _db.Set<OutboxMessage>()
            .Where(x => eligible.Contains(x.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Status, OutboxStatus.InFlight)
                .SetProperty(x => x.LockToken, workerId)
                .SetProperty(x => x.LockedAtUtc, now),
                ct);

        var items = await _db.Set<OutboxMessage>()
            .Where(x => eligible.Contains(x.Id) && x.LockToken == workerId && x.Status == OutboxStatus.InFlight)
            .OrderBy(x => x.StoredOnUtc)
            .ToListAsync(ct);

        return items;
    }
}