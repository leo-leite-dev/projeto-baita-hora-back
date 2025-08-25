using BaitaHora.Domain.Entities.Scheduling;
using BaitaHora.Domain.Features.Schedules.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> b)
    {
        b.ToTable("schedules");

        b.HasKey(x => x.Id);

        b.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        b.HasIndex(x => x.UserId)
            .IsUnique();

        b.Property(x => x.TimeZone)
            .HasMaxLength(100)
            .HasColumnName("time_zone");

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc");

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        b.HasMany(typeof(Appointment), nameof(Schedule.Appointments))
         .WithOne(nameof(Appointment.Schedule))
         .HasForeignKey(nameof(Appointment.ScheduleId))
         .OnDelete(DeleteBehavior.Cascade);

        var nav = b.Metadata.FindNavigation(nameof(Schedule.Appointments));
        if (nav is not null)
        {
            nav.SetField("_appointments");
            nav.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}