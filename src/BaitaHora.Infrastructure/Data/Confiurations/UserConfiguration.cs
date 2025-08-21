using BaitaHora.Domain.Features.Commons.ValueObjects;
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
            toProvider   => toProvider.Value,
            fromProvider => Username.Parse(fromProvider)
        );
        var emailConverter = new ValueConverter<Email, string>(
            toProvider   => toProvider.Value,
            fromProvider => Email.Parse(fromProvider)
        );

        builder.Property(u => u.Username)
            .HasConversion(usernameConverter)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.Email)
            .HasConversion(emailConverter)
            .HasMaxLength(256) 
            .IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired();

        builder.HasOne(u => u.Profile)
            .WithOne()
            .HasForeignKey<User>(u => u.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}