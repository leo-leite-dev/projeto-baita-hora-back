using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BaitaHora.Infrastructure.Data.Configurations.Customers;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> b)
    {
        b.ToTable("customers");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Ignore(x => x.DomainEvents);

        var cpfConverter = new ValueConverter<CPF, string>(
            v => v.Value,
            v => CPF.Parse(v)
        );

        var phoneConverter = new ValueConverter<Phone, string>(
            v => v.Value,
            v => Phone.Parse(v)
        );

        b.Property(x => x.Name)
            .HasColumnName("customer_name")
            .HasMaxLength(120)
            .IsRequired();

        b.Property(x => x.Phone)
            .HasColumnName("customer_phone")
            .HasConversion(phoneConverter)
            .HasMaxLength(16)
            .IsRequired();

        b.Property(x => x.Cpf)
            .HasColumnName("customer_cpf")
            .HasConversion(cpfConverter)
            .HasMaxLength(11)
            .IsRequired();

        b.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        b.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        b.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        b.Property(x => x.NoShowCount)
            .HasColumnName("no_show_count")
            .HasDefaultValue(0);

        b.Property(x => x.NoShowPenaltyTotal)
            .HasColumnName("no_show_penalty_total")
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0m);

        b.HasIndex(x => x.Cpf)
            .IsUnique()
            .HasDatabaseName("ux_customers_cpf");

        b.HasIndex(x => x.Phone)
            .IsUnique()
            .HasDatabaseName("ux_customers_phone");

        b.HasIndex(x => x.Name)
            .HasDatabaseName("ix_customers_name");
    }
}