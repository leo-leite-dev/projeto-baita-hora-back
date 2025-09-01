using BaitaHora.Domain.Features.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class CompanyServiceOfferingConfiguration : IEntityTypeConfiguration<CompanyServiceOffering>
{
    public void Configure(EntityTypeBuilder<CompanyServiceOffering> b)
    {
        b.ToTable("company_service_offerings");

        b.HasKey(x => x.Id);

        b.Property(p => p.Id)
         .ValueGeneratedNever();

        b.Property(x => x.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();

        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
        b.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc");

        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        b.ComplexProperty(x => x.Price, price =>
        {
            price.Property(m => m.Amount)
                 .HasColumnName("price_amount")
                 .HasPrecision(12, 2)
                 .IsRequired();

            price.Property(m => m.Currency)
                 .HasColumnName("price_currency")
                 .HasMaxLength(3)
                 .IsRequired();
        });

        b.HasIndex(x => new { x.CompanyId, x.Name })
            .IsUnique()
            .HasDatabaseName("ux_cso_company_name");

        b.HasIndex(x => x.CompanyId)
            .HasDatabaseName("ix_cso_company");
    }
}