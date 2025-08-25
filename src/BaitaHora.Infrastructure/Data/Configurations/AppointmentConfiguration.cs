using BaitaHora.Domain.Entities.Scheduling;
using BaitaHora.Domain.Features.Schedules.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> b)
    {
        b.ToTable("appointments");

        b.HasKey(x => x.Id);

        b.Property(x => x.ScheduleId)
            .IsRequired()
            .HasColumnName("schedule_id");

        b.Property(x => x.StartsAtUtc)
            .IsRequired()
            .HasColumnName("starts_at_utc");

        b.Property(x => x.EndsAtUtc)
            .IsRequired()
            .HasColumnName("ends_at_utc");

        b.Property(x => x.CustomerId)
            .IsRequired()
            .HasColumnName("customer_id");

        b.Property(x => x.CustomerDisplayName)
            .HasMaxLength(200)
            .HasColumnName("customer_display_name");

        b.Property(x => x.CustomerPhone)
            .HasMaxLength(30)
            .HasColumnName("customer_phone");

        b.Property(x => x.ServiceId)
            .HasColumnName("service_id");

        b.Property(x => x.Notes)
            .HasMaxLength(2000)
            .HasColumnName("notes");

        b.Property(x => x.CancellationReason)
            .HasMaxLength(500)
            .HasColumnName("cancellation_reason");

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc");

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        b.HasIndex(x => new { x.ScheduleId, x.StartsAtUtc })
            .HasDatabaseName("ix_appointments_schedule_start");


        b.HasOne(x => x.Schedule)
         .WithMany(nameof(Schedule.Appointments))
         .HasForeignKey(x => x.ScheduleId);
    }
}