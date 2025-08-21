using BaitaHora.Domain.Entities.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class CompanyPositionConfiguration : IEntityTypeConfiguration<CompanyPosition>
{
    public void Configure(EntityTypeBuilder<CompanyPosition> builder)
    {
        builder.ToTable("CompanyPositions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasColumnName("Name")
            .HasMaxLength(200)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(p => p.AccessLevel)
            .HasColumnName("AccessLevel")
            .IsRequired();

        builder.HasOne(p => p.Company)
            .WithMany(c => c.Positions)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}