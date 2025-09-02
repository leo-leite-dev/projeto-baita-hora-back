using BaitaHora.Domain.Features.Schedules.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations.Schedules;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> b)
    {
        b.ToTable("appointments");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.ScheduleId).IsRequired();
        b.Property(x => x.CustomerId).IsRequired();

        b.Property(x => x.StartsAtUtc)
            .HasColumnName("starts_at_utc")
            .HasColumnType("timestamptz")
            .IsRequired();

        b.Property(x => x.Duration)
            .HasColumnName("duration")
            .HasColumnType("interval")
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasColumnType("varchar(20)")
            .HasConversion<string>()
            .HasDefaultValue(AppointmentStatus.Pending)
            .IsRequired();

        b.ToTable(tb =>
        {
            tb.HasCheckConstraint(
                "ck_appointments_duration_positive",
                "duration > interval '0 seconds'");

            tb.HasCheckConstraint(
                "ck_appointments_status_valid",
                "status in ('Pending','Cancelled','Completed')");
        });

        b.HasIndex(x => x.ScheduleId)
            .HasDatabaseName("ix_appointments_schedule");

        b.HasIndex(x => new { x.ScheduleId, x.StartsAtUtc })
            .IsUnique()
            .HasDatabaseName("ux_appointments_schedule_start");

        b.HasOne<Schedule>()
            .WithMany(s => s.Appointments)
            .HasForeignKey(x => x.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}