using BaitaHora.Domain.Companies.ValueObjects; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Commons.ValueObjects;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");
        builder.HasKey(c => c.Id);

        var companyNameConverter = new ValueConverter<CompanyName, string>(
            toProvider   => toProvider.Value,
            fromProvider => CompanyName.Parse(fromProvider)
        );
        var cnpjConverter = new ValueConverter<CNPJ, string>(
            toProvider   => toProvider.Value,
            fromProvider => CNPJ.Parse(fromProvider)
        );
        var emailConverter = new ValueConverter<Email, string>(
            toProvider   => toProvider.Value,
            fromProvider => Email.Parse(fromProvider)
        );
        var phoneConverter = new ValueConverter<Phone, string>(
            toProvider   => toProvider.Value,
            fromProvider => Phone.Parse(fromProvider)
        );

        builder.Property(c => c.CompanyName)
            .HasConversion(companyNameConverter)
            .HasMaxLength(120)
            .IsRequired();

        builder.HasIndex(c => c.CompanyName)
            .IsUnique();

        builder.Property(c => c.Cnpj)
            .HasConversion(cnpjConverter)
            .HasMaxLength(14)
            .IsRequired();

        builder.HasIndex(c => c.Cnpj)
            .IsUnique();

        builder.Property(c => c.Email)
            .HasConversion(emailConverter)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasConversion(phoneConverter)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(c => c.TradeName)
            .HasMaxLength(120);

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.OwnsOne(c => c.Address, addr =>
        {
            addr.Property(a => a.Street).HasMaxLength(120).IsRequired();
            addr.Property(a => a.Number).HasMaxLength(20).IsRequired();
            addr.Property(a => a.Neighborhood).HasMaxLength(80).IsRequired();
            addr.Property(a => a.City).HasMaxLength(80).IsRequired();
            addr.Property(a => a.State).HasMaxLength(2).IsRequired();
            addr.Property(a => a.ZipCode).HasMaxLength(10).IsRequired();
            addr.Property(a => a.Complement).HasMaxLength(120);
        });
        builder.Navigation(c => c.Address).IsRequired();

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Company)
            .HasForeignKey(m => m.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Positions)
            .WithOne(p => p.Company)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Image)
            .WithOne(i => i.Company)
            .HasForeignKey<CompanyImage>(i => i.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}