using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        var usernameConverter = new ValueConverter<Username, string>(
            v => v.Value,
            v => Username.Parse(v));

        var emailConverter = new ValueConverter<Email, string>(
            v => v.Value,
            v => Email.Parse(v));

        builder.Property(u => u.Username)
            .HasConversion(usernameConverter)
            .HasMaxLength(50)
            .HasColumnType("citext")
            .HasColumnName("username")
            .IsRequired();

        builder.Property(u => u.UserEmail)
            .HasConversion(emailConverter)
            .HasMaxLength(256)
            .HasColumnType("citext")
            .HasColumnName("email")
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(u => u.PasswordResetToken)
            .HasColumnName("pwd_reset_token")
            .HasMaxLength(200);

        builder.Property(u => u.PasswordResetTokenExpiresAt)
            .HasColumnName("pwd_reset_expires_at");

        builder.Property(u => u.TokenVersion)
            .HasColumnName("token_version")
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasOne(u => u.Profile)
            .WithOne()
            .HasForeignKey<User>(u => u.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("ux_users_username");

        builder.HasIndex(u => u.UserEmail)
            .IsUnique()
            .HasDatabaseName("ux_users_email");
    }
}