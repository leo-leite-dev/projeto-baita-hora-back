using BaitaHora.Domain.Features.Common.ValueObjects; 
using BaitaHora.Domain.Features.Customers;
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

        var usernameConverter = new ValueConverter<Username, string>(
            v => v.Value,
            v => Username.Parse(v)
        );

        var cpfConverter = new ValueConverter<CPF, string>(
            v => v.Value,
            v => CPF.Parse(v)
        );

        var phoneConverter = new ValueConverter<Phone, string>(
            v => v.Value,
            v => Phone.Parse(v)
        );

        b.Property(x => x.CustomerName)
            .HasColumnName("customer_name")
            .HasConversion(usernameConverter)
            .HasMaxLength(50)
            .IsRequired();

        b.Property(x => x.CustomerPhone)
            .HasColumnName("customer_phone")
            .HasConversion(phoneConverter)
            .HasMaxLength(16)
            .IsRequired();

        b.Property(x => x.CustomerCpf)
            .HasColumnName("customer_cpf")
            .HasConversion(cpfConverter)
            .HasMaxLength(11)
            .IsRequired();

        b.HasIndex(x => x.CustomerCpf)
            .IsUnique()
            .HasDatabaseName("ux_customers_cpf");

        b.HasIndex(x => x.CustomerPhone)
            .IsUnique()
            .HasDatabaseName("ux_customers_phone");

        b.HasIndex(x => x.CustomerName)
            .HasDatabaseName("ix_customers_name");
    }
}