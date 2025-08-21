using BaitaHora.Domain.Entities.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
{
    public void Configure(EntityTypeBuilder<CompanyMember> builder)
    {
        builder.ToTable("CompanyMembers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Role).IsRequired();

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId);

        builder.HasOne(m => m.PrimaryPosition)
            .WithMany()
            .HasForeignKey(m => m.PrimaryPositionId);
    }
}
