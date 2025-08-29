using BaitaHora.Domain.Features.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class CompanyPositionConfiguration : IEntityTypeConfiguration<CompanyPosition>
{
    public void Configure(EntityTypeBuilder<CompanyPosition> b)
    {
        b.ToTable("company_positions");

        b.HasKey(p => p.Id);

        b.Property(p => p.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();

        b.Property(p => p.PositionName)
            .HasColumnName("name")
            .HasMaxLength(60)
            .HasColumnType("citext")
            .IsRequired();

        b.Property(p => p.AccessLevel)
            .HasColumnName("access_level")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired();

        b.Property(p => p.PermissionMask)
            .HasColumnName("permission_mask")
            .HasConversion<int>()
            .IsRequired();

        b.Property(p => p.IsSystem)
            .HasColumnName("is_system")
            .HasDefaultValue(false)
            .IsRequired();

        b.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        b.Property(p => p.CreatedAtUtc)
            .HasColumnName("created_at_utc");

        b.Property(p => p.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        b.Navigation(p => p.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        b.HasOne<Company>()
            .WithMany(c => c.Positions)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(p => p.ServiceOfferings)
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("company_position_service_offerings");
                j.Property<Guid>("CompanyPositionId").HasColumnName("position_id");
                j.Property<Guid>("CompanyServiceOfferingId").HasColumnName("service_offering_id");
                j.HasKey("CompanyPositionId", "CompanyServiceOfferingId");

                j.HasIndex("CompanyPositionId").HasDatabaseName("ix_cpso_position");
                j.HasIndex("CompanyServiceOfferingId").HasDatabaseName("ix_cpso_service");
            });

        b.HasIndex(p => new { p.CompanyId, p.PositionName })
            .IsUnique()
            .HasDatabaseName("ux_company_positions_companyid_name");

        b.HasIndex(p => p.CompanyId)
            .HasDatabaseName("ix_company_positions_companyid");
    }
}