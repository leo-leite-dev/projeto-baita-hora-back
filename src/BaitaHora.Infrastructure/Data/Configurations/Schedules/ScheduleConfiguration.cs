using BaitaHora.Domain.Features.Schedules.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations.Schedules;

public sealed class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> b)
    {
        b.ToTable("schedules");

        b.HasKey(s => s.Id);
        b.Property(s => s.Id).ValueGeneratedNever();

        b.Property(s => s.MemberId)
            .HasColumnName("member_id")
            .IsRequired();

        b.HasMany(s => s.Appointments)
            .WithOne()
            .HasForeignKey(a => a.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(s => s.MemberId)
            .HasDatabaseName("ix_schedules_member");
    }
}