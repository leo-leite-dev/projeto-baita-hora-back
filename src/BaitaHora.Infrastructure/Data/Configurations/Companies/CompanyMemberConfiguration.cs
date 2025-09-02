using BaitaHora.Domain.Features.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations.Companies;

public sealed class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
{
    public void Configure(EntityTypeBuilder<CompanyMember> b)
    {
        b.ToTable("company_members");
        b.HasKey(m => m.Id);

        b.Property(m => m.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();

        b.Property(m => m.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        b.Property(m => m.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired();

        b.Property(m => m.PrimaryPositionId)
            .HasColumnName("primary_position_id");

        b.Property(m => m.DirectPermissionMask)
            .HasColumnName("direct_permission_mask")
            .HasConversion<int>()
            .IsRequired();

        b.Property(m => m.JoinedAt)
            .HasColumnName("joined_at_utc")
            .IsRequired();

        b.Property(m => m.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        b.HasOne(m => m.PrimaryPosition)
            .WithMany(p => p.Members)
            .HasForeignKey(m => m.PrimaryPositionId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(m => new { m.CompanyId, m.UserId })
            .IsUnique()
            .HasDatabaseName("ux_company_members_company_user");

        b.HasIndex(m => m.CompanyId)
            .HasDatabaseName("ix_company_members_company");

        b.HasIndex(m => m.UserId)
            .HasDatabaseName("ix_company_members_user");

        b.HasIndex(m => m.PrimaryPositionId)
            .HasDatabaseName("ix_company_members_primary_position");
    }
}