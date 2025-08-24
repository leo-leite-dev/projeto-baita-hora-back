using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
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
            toProvider => toProvider.Value,
            fromProvider => Username.Parse(fromProvider)
        );

        var emailConverter = new ValueConverter<Email, string>(
            toProvider => toProvider.Value,
            fromProvider => Email.Parse(fromProvider)
        );

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Username)
            .HasConversion(usernameConverter)
            .HasMaxLength(50)
            .HasColumnType("citext")   
            .IsRequired();

        builder.Property(u => u.UserEmail)
            .HasConversion(emailConverter)
            .HasMaxLength(256)
            .HasColumnType("citext")  
            .IsRequired();

        builder.Property(u => u.PasswordHash)
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