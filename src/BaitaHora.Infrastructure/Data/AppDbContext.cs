using BaitaHora.Domain.Companies.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Users.ValueObjects;
using BaitaHora.Infrastructure.Persistence.Entities;
using BaitaHora.Infrastructure.Data.Outbox;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Application.Abstractions.Data;

namespace BaitaHora.Infrastructure.Data;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyImage> CompanyImages => Set<CompanyImage>();
    public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();
    public DbSet<CompanyPosition> CompanyPositions => Set<CompanyPosition>();
    public DbSet<LoginSession> LoginSessions => Set<LoginSession>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --------- Converters ---------
        var emailConverter = new ValueConverter<Email, string>(
            v => v.Value,
            v => Email.Parse(v)
        );

        var cpfConverter = new ValueConverter<CPF, string>(
            cpf => cpf.Value,
            value => CPF.Parse(value)
        );

        var usernameConverter = new ValueConverter<Username, string>(
            u => u.Value,
            value => Username.Parse(value)
        );

        var cnpjConverter = new ValueConverter<CNPJ, string>(
            cnpj => cnpj.Value,
            value => CNPJ.Parse(value)
        );

        var companyNameConverter = new ValueConverter<CompanyName, string>(
            n => n.Value,
            value => CompanyName.Parse(value)
        );

        var rgConverter = new ValueConverter<RG?, string?>(
            rg => rg.HasValue ? rg.Value.Value : null,
            value => value == null ? null : RG.Parse(value)
        );

        // --------- Entidades ---------
        modelBuilder.Entity<User>()
            .Property(u => u.UserEmail)
            .HasConversion(emailConverter)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();


        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.Property(p => p.Cpf)
                  .HasConversion(cpfConverter);

            entity.Property(p => p.Rg)
                  .HasConversion(rgConverter);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.Property(c => c.Cnpj)
                  .HasConversion(cnpjConverter);

            entity.Property(c => c.CompanyName)
                  .HasConversion(companyNameConverter);
        });

        // aplica configs adicionais (se tiver IEntityTypeConfiguration<>)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
