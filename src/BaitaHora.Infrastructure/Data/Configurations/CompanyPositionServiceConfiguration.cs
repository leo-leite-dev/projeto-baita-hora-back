using BaitaHora.Domain.Features.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class CompanyPositionServiceConfiguration : IEntityTypeConfiguration<CompanyPositionService>
{
    public void Configure(EntityTypeBuilder<CompanyPositionService> b)
    {
        b.ToTable("company_position_services");

        b.HasKey(x => x.Id);

        b.Property(x => x.PositionId)
            .IsRequired()
            .HasColumnName("position_id");

        b.Property(x => x.ServiceId)
            .IsRequired()
            .HasColumnName("service_id");

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc");

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        b.HasIndex(x => x.ServiceId);
        b.HasIndex(x => x.PositionId);

        b.HasIndex(x => new { x.PositionId, x.ServiceId }).IsUnique();
    }
}