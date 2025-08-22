using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class CompanyPositionConfiguration : IEntityTypeConfiguration<CompanyPosition>
{
    public void Configure(EntityTypeBuilder<CompanyPosition> builder)
    {
        builder.ToTable("company_positions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CompanyId)
            .HasColumnName("company_id")
            .IsRequired();

        builder.Property(p => p.PositionName)
            .HasColumnName("name")
            .HasMaxLength(60)
            .HasColumnType("citext")
            .IsRequired();

        builder.Property(p => p.AccessLevel)
            .HasColumnName("access_level")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(p => p.IsSystem)
            .HasColumnName("is_system")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasOne(p => p.Company)
            .WithMany(c => c.Positions)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.CompanyId, p.PositionName })
            .IsUnique()
            .HasDatabaseName("ux_company_positions_companyid_name");
    }
}