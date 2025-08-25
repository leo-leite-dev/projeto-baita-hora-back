using BaitaHora.Domain.Features.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class CompanyServiceConfiguration : IEntityTypeConfiguration<CompanyService>
{
    public void Configure(EntityTypeBuilder<CompanyService> b)
    {
        b.ToTable("company_services");
        b.HasKey(x => x.Id);

        b.Property(x => x.CompanyId)
            .IsRequired()
            .HasColumnName("company_id");

        b.Property(x => x.ServiceName)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("citext")
            .HasColumnName("service_name");

        b.ComplexProperty(x => x.Price, price =>
        {
            price.Property(p => p.Amount)
                 .HasColumnName("price_amount")
                 .HasColumnType("numeric(12,2)")
                 .IsRequired();

            price.Property(p => p.Currency)
                 .HasColumnName("price_currency")
                 .HasMaxLength(3)
                 .IsRequired();
        });

        b.Property(x => x.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        b.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
        b.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc");

        b.HasIndex(x => x.CompanyId);

        b.HasIndex(x => new { x.CompanyId, x.ServiceName })
         .IsUnique()
         .HasDatabaseName("ux_company_services_companyid_servicename");

        b.HasMany<CompanyPositionService>(nameof(CompanyService.PositionLinks))
         .WithOne()
         .HasForeignKey(l => l.ServiceId)
         .OnDelete(DeleteBehavior.Cascade);

        var nav = b.Metadata.FindNavigation(nameof(CompanyService.PositionLinks));
        if (nav is not null)
        {
            nav.SetField("_positionLinks");
            nav.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}