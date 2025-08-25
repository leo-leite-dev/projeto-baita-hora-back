using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");
        builder.HasKey(c => c.Id);

        var companyNameConverter = new ValueConverter<CompanyName, string>(
            v => v.Value,
            v => CompanyName.Parse(v)
        );

        var cnpjConverter = new ValueConverter<CNPJ, string>(
            v => v.Value,
            v => CNPJ.Parse(v)
        );

        var emailConverter = new ValueConverter<Email, string>(
            v => v.Value,
            v => Email.Parse(v)
        );

        var phoneConverter = new ValueConverter<Phone, string>(
            v => v.Value,
            v => Phone.Parse(v)
        );

        builder.Property(c => c.CompanyName)
               .HasConversion(companyNameConverter)
               .HasMaxLength(150)
               .HasColumnType("citext")    
               .IsRequired();

        builder.Property(c => c.Cnpj)
               .HasConversion(cnpjConverter)
               .HasMaxLength(18)        
               .IsRequired();

        builder.Property(c => c.CompanyEmail)
               .HasConversion(emailConverter)
               .HasMaxLength(256)
               .HasColumnType("citext")  
               .IsRequired();

        builder.Property(c => c.CompanyPhone)
               .HasConversion(phoneConverter)
               .HasMaxLength(32)
               .IsRequired();

        builder.Property(c => c.TradeName)
               .HasMaxLength(150);

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

        builder.HasOne(c => c.Image)
               .WithOne(i => i.Company)
               .HasForeignKey<CompanyImage>(i => i.CompanyId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Members)
               .HasField("_members")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(c => c.Positions)
               .HasField("_companyPositions")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(c => c.CompanyName)
               .IsUnique()
               .HasDatabaseName("ux_companies_name");

        builder.HasIndex(c => c.Cnpj)
               .IsUnique()
               .HasDatabaseName("ux_companies_cnpj");

        builder.HasIndex(c => c.CompanyEmail)
               .IsUnique()
               .HasDatabaseName("ux_companies_email");
    }
}