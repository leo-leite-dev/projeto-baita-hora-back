using BaitaHora.Domain.Features.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations.Companies;

public sealed class PositionConfiguration : IEntityTypeConfiguration<CompanyPosition>
{
    public void Configure(EntityTypeBuilder<CompanyPosition> b)
    {
        b.ToTable("company_positions");

        b.HasKey(p => p.Id);

        b.Property(p => p.Id)
         .ValueGeneratedNever();

        b.Property(p => p.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();

        b.Property(p => p.Name)
            .HasColumnName("position_name")
            .HasColumnType("citext")
            .HasMaxLength(60)
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

        b.HasOne<Company>()
            .WithMany(c => c.Positions)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(p => p.ServiceOfferings)
         .WithMany()
         .UsingEntity<Dictionary<string, object>>(
             "company_position_service_offerings",
             right => right
                 .HasOne<CompanyServiceOffering>()
                 .WithMany()
                 .HasForeignKey("company_service_offering_id")
                 .OnDelete(DeleteBehavior.Cascade),
             left => left
                 .HasOne<CompanyPosition>()
                 .WithMany()
                 .HasForeignKey("position_id")
                 .OnDelete(DeleteBehavior.Cascade),
             join =>
             {
                 join.ToTable("company_position_service_offerings");

                 join.HasKey("position_id", "company_service_offering_id");

                 join.IndexerProperty<Guid>("position_id")
                     .HasColumnName("position_id");

                 join.IndexerProperty<Guid>("company_service_offering_id")
                     .HasColumnName("company_service_offering_id");

                 join.HasIndex("position_id").HasDatabaseName("ix_cpso_position");
                 join.HasIndex("company_service_offering_id").HasDatabaseName("ix_cpso_company_service");
             });

        b.Navigation(p => p.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        b.Navigation(p => p.ServiceOfferings)
            .HasField("_serviceOfferings")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        b.HasIndex(p => new { p.CompanyId, p.Name })
            .IsUnique()
            .HasDatabaseName("ux_company_positions_companyid_name");

        b.HasIndex(p => p.CompanyId)
            .HasDatabaseName("ix_company_positions_companyid");
    }
}