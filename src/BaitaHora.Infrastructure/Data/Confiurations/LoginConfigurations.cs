using BaitaHora.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class LoginSessionConfig : IEntityTypeConfiguration<LoginSession>
{
    public void Configure(EntityTypeBuilder<LoginSession> b)
    {
        b.ToTable("login_sessions");

        b.HasKey(x => x.Id);

        b.Property(x => x.UserId).IsRequired();

        b.Property(x => x.RefreshTokenHash)
            .IsRequired()
            .HasMaxLength(512);

        b.Property(x => x.RefreshTokenExpiresAtUtc)
            .IsRequired();

        b.Property(x => x.IsRevoked)
            .IsRequired();

        b.Property(x => x.Ip)
            .HasMaxLength(64);

        b.Property(x => x.UserAgent)
            .HasMaxLength(256);

        b.Property(x => x.CreatedAtUtc)
            .IsRequired();

        b.HasIndex(x => x.UserId);
        b.HasIndex(x => new { x.UserId, x.IsRevoked });
        b.HasIndex(x => x.RefreshTokenHash).HasDatabaseName("ix_login_sessions_rthash");
    }
}