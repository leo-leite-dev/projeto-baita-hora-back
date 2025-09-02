using BaitaHora.Domain.Features.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations.Companies;

public sealed class CompanyImageConfiguration : IEntityTypeConfiguration<CompanyImage>
{
    public void Configure(EntityTypeBuilder<CompanyImage> builder)
    {
        builder.ToTable("CompanyImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Url)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(i => i.Company)
            .WithOne(c => c.Image)
            .HasForeignKey<CompanyImage>(i => i.CompanyId);
    }
}