using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Outbox;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> b)
    {
        b.ToTable("outbox_messages");
        b.HasKey(x => x.Id);

        b.Property(x => x.Type).HasMaxLength(200).IsRequired();
        b.Property(x => x.Payload).IsRequired();

        b.Property(x => x.Status).HasConversion<short>();
        b.Property(x => x.AttemptCount);
        b.Property(x => x.NextAttemptUtc);
        b.Property(x => x.LastError);
        b.Property(x => x.LockToken);
        b.Property(x => x.LockedAtUtc);

        b.HasIndex(x => x.StoredOnUtc);
        b.HasIndex(x => new { x.Status, x.NextAttemptUtc });
    }
}