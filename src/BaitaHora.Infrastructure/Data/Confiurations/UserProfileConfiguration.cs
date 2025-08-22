using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FullName)
            .HasMaxLength(200)
            .IsRequired();

        var cpfConverter = new ValueConverter<CPF, string>(
            toProvider => toProvider.Value,
            fromProvider => CPF.Parse(fromProvider)
        );

        var rgConverter = new ValueConverter<RG?, string?>(
            toProvider => toProvider.HasValue ? toProvider.Value.Value : null,
            fromProvider => string.IsNullOrEmpty(fromProvider) ? null : RG.Parse(fromProvider)
        );

        var phoneConverter = new ValueConverter<Phone, string>(
            toProvider => toProvider.Value,
            fromProvider => Phone.Parse(fromProvider)
        );

        builder.Property(p => p.Cpf)
            .HasConversion(cpfConverter)
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(p => p.Rg)
            .HasConversion(rgConverter)
            .HasMaxLength(20);

        builder.Property(p => p.UserPhone)
            .HasConversion(phoneConverter)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(p => p.BirthDate)
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

        builder.Property(x => x.Rg)
            .HasMaxLength(20)
            .HasColumnName("rg");

        builder.HasIndex(x => x.Rg)
            .HasDatabaseName("ux_user_profiles_rg")
            .IsUnique()
            .HasFilter("\"rg\" IS NOT NULL");
    }
}