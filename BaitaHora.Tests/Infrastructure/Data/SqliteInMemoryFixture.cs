using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BaitaHora.Infrastructure.Data;

namespace BaitaHora.Tests;

public sealed class SqliteInMemoryFixture : IAsyncLifetime
{
    private SqliteConnection _conn = default!;
    public DbContextOptions<AppDbContext> Options { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        _conn = new SqliteConnection("DataSource=:memory:");
        await _conn.OpenAsync();

        Options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_conn)
            // Habilite logs se quiser ver SQL no teste:
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        using var ctx = new AppDbContext(Options);
        await ctx.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _conn.DisposeAsync();
    }

    public AppDbContext CreateContext() => new AppDbContext(Options);
}
