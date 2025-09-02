using BaitaHora.Domain.Features.Schedules.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations.Schedules;

public sealed class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> b)
    {
        b.ToTable("schedules");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.MemberId)
         .IsRequired();

        b.HasIndex(x => x.MemberId)
         .IsUnique()
         .HasDatabaseName("ux_schedules_user");

        b.HasMany(x => x.Appointments)
         .WithOne()
         .HasForeignKey(a => a.ScheduleId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}