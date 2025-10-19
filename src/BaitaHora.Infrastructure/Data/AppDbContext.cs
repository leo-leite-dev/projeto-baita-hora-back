using Microsoft.EntityFrameworkCore;
using BaitaHora.Domain.Features.Users.Entities;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Persistence.Entities;
using BaitaHora.Infrastructure.Data.Outbox;
using BaitaHora.Application.Abstractions.Data;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Domain.Features.Customers;

namespace BaitaHora.Infrastructure.Data;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyImage> CompanyImages => Set<CompanyImage>();
    public DbSet<CompanyMember> Members => Set<CompanyMember>();
    public DbSet<CompanyPosition> Positions => Set<CompanyPosition>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<LoginSession> LoginSessions => Set<LoginSession>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}