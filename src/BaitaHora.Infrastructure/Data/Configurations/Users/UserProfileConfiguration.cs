using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BaitaHora.Infrastructure.Data.Configurations.Users;

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasColumnName("profile_name")
            .HasMaxLength(120)
            .IsRequired();

        var cpfConverter = new ValueConverter<CPF, string>(
            v => v.Value,
            v => CPF.Parse(v));

        var rgConverter = new ValueConverter<RG?, string?>(
            v => v.HasValue ? v.Value.Value : null,
            v => string.IsNullOrWhiteSpace(v) ? null : RG.Parse(v!));

        var phoneConverter = new ValueConverter<Phone, string>(
            v => v.Value,
            v => Phone.Parse(v));

        var dobConverter = new ValueConverter<DateOfBirth?, DateOnly?>(
            v => v.HasValue ? v.Value.Value : null,
            v => v.HasValue ? DateOfBirth.Parse(v.Value) : null);

        builder.Property(p => p.Cpf)
            .HasConversion(cpfConverter)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(p => p.Rg)
            .HasConversion(rgConverter)
            .HasMaxLength(20)
            .HasColumnName("rg");

        builder.Property(p => p.UserPhone)
            .HasConversion(phoneConverter)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(p => p.BirthDate)
            .HasConversion(dobConverter)
            .HasColumnType("date");

        builder.OwnsOne(p => p.Address, addr =>
        {
            addr.Property(a => a.Street).HasMaxLength(120).IsRequired();
            addr.Property(a => a.Number).HasMaxLength(20).IsRequired();
            addr.Property(a => a.Neighborhood).HasMaxLength(80).IsRequired();
            addr.Property(a => a.City).HasMaxLength(80).IsRequired();
            addr.Property(a => a.State).HasMaxLength(2).IsRequired();
            addr.Property(a => a.ZipCode).HasMaxLength(10).IsRequired();
            addr.Property(a => a.Complement).HasMaxLength(120);
        });
        builder.Navigation(p => p.Address).IsRequired();

        builder.Property(p => p.ProfileImageUrl)
            .HasMaxLength(512);

        builder.HasIndex(p => p.Cpf)
            .HasDatabaseName("ux_user_profiles_cpf")
            .IsUnique();

        builder.HasIndex(p => p.Rg)
            .HasDatabaseName("ux_user_profiles_rg")
            .IsUnique()
            .HasFilter("\"rg\" IS NOT NULL");

        builder.HasIndex(p => p.UserPhone)
            .HasDatabaseName("ux_user_profiles_phone")
            .IsUnique();
    }
}