using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Schedules.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaitaHora.Infrastructure.Data.Configurations.Schedules;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> b)
    {
        b.ToTable("appointments");

        b.HasKey(a => a.Id);
        b.Property(a => a.Id).ValueGeneratedNever();

        b.Property(a => a.ScheduleId).HasColumnName("schedule_id");
        b.Property(a => a.CustomerId).HasColumnName("customer_id");

        b.Property(a => a.StartsAtUtc)
         .HasColumnName("starts_at_utc")
         .HasColumnType("timestamptz")
         .IsRequired();

        b.Property(a => a.Duration)
         .HasColumnName("duration")
         .HasColumnType("interval")
         .IsRequired();

        b.Property(a => a.Status)
         .HasColumnName("status")
         .HasConversion<string>()
         .HasColumnType("varchar(20)")
         .IsRequired();

        b.Property(a => a.AttendanceStatus)
         .HasColumnName("attendance_status")
         .HasConversion<string>()
         .HasColumnType("varchar(20)")
         .IsRequired();

        b.HasMany(a => a.ServiceOfferings)
         .WithMany()
         .UsingEntity<Dictionary<string, object>>(
            "appointment_service_offerings",
            j => j
                .HasOne<CompanyServiceOffering>()
                .WithMany()
                .HasForeignKey("service_offering_id")
                .OnDelete(DeleteBehavior.Cascade),
            j => j
                .HasOne<Appointment>()
                .WithMany()
                .HasForeignKey("appointment_id")
                .OnDelete(DeleteBehavior.Cascade),
            j =>
            {
                j.ToTable("appointment_service_offerings");
                j.HasKey("appointment_id", "service_offering_id");
            });
    }
}