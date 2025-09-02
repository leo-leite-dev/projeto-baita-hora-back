using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using BaitaHora.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;

public class CompanyServiceOffering_ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CompanyServiceOffering_ApiTests(WebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("JwtOptions__Issuer", "test-issuer");
        Environment.SetEnvironmentVariable("JwtOptions__Audience", "test-audience");
        Environment.SetEnvironmentVariable("JwtOptions__Secret", "super-secret-test-key-32chars-min-OK!!");
        Environment.SetEnvironmentVariable("JwtOptions__AccessTokenMinutes", "60");
        Environment.SetEnvironmentVariable("JwtOptions__RefreshTokenDays", "7");
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "Host=localhost;Database=test;Username=u;Password=p");

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<IDbContextFactory<AppDbContext>>();
                services.RemoveAll<AppDbContext>();
                services.AddEntityFrameworkInMemoryDatabase();
                services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("E2E_DB"));
            });
        });
    }

    [Fact(Skip = "Conflito de EF providers (Npgsql+InMemory) vindo da Infra. Habilitar 'modo Testing' no Program/Infra para registrar apenas InMemory.")]
    public async Task POST_ServiceOffering_Deve_Criar_E_Listar()
    {
        var client = _factory.CreateClient();
        var companyId = Guid.Parse("0e1f2a08-abb0-4aa1-a378-851a599cb88b");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        var loginBody = new { companyId, identify = "leonardo.silva", password = "SenhaForte@123" };
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", loginBody);
        Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);

        var token = await loginResp.Content.ReadFromJsonAsync<AuthTokenDto>();
        Assert.NotNull(token);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);

        var body = new { serviceOfferingName = "Barba completa", currency = "BRL", amount = 59.90m };
        var createResp = await client.PostAsJsonAsync($"/api/companies/{companyId}/service-offerings", body);
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
    }

    private sealed record AuthTokenDto(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc, Guid UserId, string Username, string[] Roles);
}