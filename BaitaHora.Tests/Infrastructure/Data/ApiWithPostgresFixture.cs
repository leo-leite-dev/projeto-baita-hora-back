// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using BaitaHora.Infrastructure.Data;
// using System.ComponentModel;


// public sealed class ApiWithPostgresFixture : IAsyncLifetime
// {
//     private IContainer _pg = default!;
//     public string ConnectionString { get; private set; } = default!;
//     public TestApiFactory Factory { get; private set; } = default!;
//     public HttpClient Client { get; private set; } = default!;

//     public async Task InitializeAsync()
//     {
//         _pg = new ContainerBuilder()
//             .WithImage("postgres:16-alpine")
//             .WithEnvironment("POSTGRES_USER", "test")
//             .WithEnvironment("POSTGRES_PASSWORD", "test")
//             .WithEnvironment("POSTGRES_DB", "baitahora_test")
//             .WithPortBinding(54329, 5432) // usa 54329 no host pra não conflitar com teu 5432 local
//             .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
//             .Build();

//         await _pg.StartAsync();

//         ConnectionString =
//             "Host=localhost;Port=54329;Database=baitahora_test;Username=test;Password=test;Include Error Detail=true";

//         Factory = new TestApiFactory(ConnectionString);
//         Client = Factory.CreateClient();

//         using var scope = Factory.Services.CreateScope();
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//         await db.Database.MigrateAsync();
//     }

//     public async Task DisposeAsync()
//     {
//         Client?.Dispose();
//         Factory?.Dispose();
//         if (_pg is not null) await _pg.DisposeAsync();
//     }

//     public async Task TruncateAllAsync()
//     {
//         await using var cn = new Npgsql.NpgsqlConnection(ConnectionString);
//         await cn.OpenAsync();
//         var sql = @"
// DO $$
// DECLARE s text;
// BEGIN
//   SELECT string_agg(format('%I.%I', schemaname, tablename), ', ')
//     INTO s
//   FROM pg_tables
//   WHERE schemaname = 'public';

//   IF s IS NOT NULL THEN
//     EXECUTE 'TRUNCATE TABLE ' || s || ' RESTART IDENTITY CASCADE';
//   END IF;
// END $$;";
//         await using var cmd = new Npgsql.NpgsqlCommand(sql, cn);
//         await cmd.ExecuteNonQueryAsync();
//     }
// }

// public sealed class TestApiFactory : WebApplicationFactory<Program> // ⬅️ usa o Program da tua API
// {
//     private readonly string _conn;
//     public TestApiFactory(string conn) => _conn = conn;

//     protected override IHost CreateHost(IHostBuilder builder)
//     {
//         builder.ConfigureAppConfiguration(cfg =>
//         {
//             var mem = new Dictionary<string, string?>
//             {
//                 ["ConnectionStrings:DefaultConnection"] = _conn,
//                 ["DefaultConnection"] = _conn
//             };
//             cfg.AddInMemoryCollection(mem!);
//         });

//         builder.ConfigureServices(services =>
//         {
//             var descriptors = services
//                 .Where(s => s.ServiceType == typeof(DbContextOptions<AppDbContext>))
//                 .ToList();
//             foreach (var d in descriptors) services.Remove(d);

//             services.AddDbContext<AppDbContext>(opt =>
//                 opt.UseNpgsql(_conn));
//         });

//         return base.CreateHost(builder);
//     }
// }
