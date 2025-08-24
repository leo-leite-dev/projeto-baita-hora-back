using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BaitaHora.Application.Common;
using BaitaHora.Application.Features.Auth.Commands;
using BaitaHora.Application.Features.Auth.DTOs.Responses;
using BaitaHora.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Builder;           // <- UseRouting / UseEndpoints
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;              // <- WriteAsync
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace BaitaHora.Api.Tests
{
    /// <summary>
    /// Fixture de ambiente: garante que as variáveis de ambiente necessárias
    /// (JwtOptions, WhatsApp) estão definidas ANTES do host da API subir.
    /// </summary>
    public sealed class TestEnvironment
    {
        public TestEnvironment()
        {
            // Ambiente de testes
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            // JwtOptions exigidos no Program.cs
            Environment.SetEnvironmentVariable("JwtOptions__Secret",   "12345678901234567890123456789012");
            Environment.SetEnvironmentVariable("JwtOptions__Issuer",   "test");
            Environment.SetEnvironmentVariable("JwtOptions__Audience", "test");

            // Outras configs usadas no bootstrap
            Environment.SetEnvironmentVariable("WhatsApp__BaseUrl", "http://localhost");
        }
    }

    [CollectionDefinition("Env setup")]
    public sealed class EnvCollection : ICollectionFixture<TestEnvironment> { }

    public class ProgramSmokeTests : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            // 1) Ajustes de serviços (DbContext InMemory + ponte DbContext -> AppDbContext)
            builder.ConfigureServices(services =>
            {
                var toRemove = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                    .ToList();

                foreach (var d in toRemove)
                    services.Remove(d);

                services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("api-smoke-tests"));

                // Ponte: para quem injeta DbContext (behaviors/outbox)
                services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());
            });

            // 2) Endpoint de teste via routing "clássico"
            builder.Configure(app =>
            {
                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/__smoke", async context =>
                    {
                        context.Response.ContentType = "text/plain";
                        await context.Response.WriteAsync("ok");
                    });
                });
            });
        }
    }

    [Collection("Env setup")]
    public class DomainPipelineResolutionTests : IClassFixture<ProgramSmokeTests>
    {
        private readonly ProgramSmokeTests _factory;
        private readonly ITestOutputHelper _output;

        public DomainPipelineResolutionTests(ProgramSmokeTests factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Fact]
        public void Container_DeveResolver_DomainEventsBehavior_ComDispatcher()
        {
            using var scope = _factory.Services.CreateScope();
            var sp = scope.ServiceProvider;

            var behaviors = sp.GetServices<
                IPipelineBehavior<RegisterOwnerWithCompanyCommand, Result<RegisterOwnerWithCompanyResponse>>>();

            Assert.NotEmpty(behaviors);
        }

        [Fact]
        public async Task SmokeEndpoint_DeveRetornar_200_OK()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/__smoke");

            if ((int)resp.StatusCode >= 500)
            {
                var body = await resp.Content.ReadAsStringAsync();
                _output.WriteLine($"[__smoke] Status: {(int)resp.StatusCode} {resp.StatusCode}");
                _output.WriteLine("[__smoke] Body:");
                _output.WriteLine(body);
            }

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal("ok", await resp.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Swagger_DeveNaoQuebrar_Pipeline()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/swagger/index.html");

            if ((int)resp.StatusCode >= 500)
            {
                var body = await resp.Content.ReadAsStringAsync();
                _output.WriteLine($"[/swagger/index.html] Status: {(int)resp.StatusCode} {resp.StatusCode}");
                _output.WriteLine("[/swagger/index.html] Body:");
                _output.WriteLine(body);
            }

            // Considera "pipeline ok" se NÃO for erro de servidor.
            Assert.DoesNotContain(resp.StatusCode, new[]
            {
                HttpStatusCode.InternalServerError,   // 500
                HttpStatusCode.BadGateway,            // 502
                HttpStatusCode.ServiceUnavailable     // 503
            });
        }

        [Fact]
        public async Task Raiz_DeveResponder_SemErroDeServidor()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/");

            if ((int)resp.StatusCode >= 500)
            {
                var body = await resp.Content.ReadAsStringAsync();
                _output.WriteLine($"[/] Status: {(int)resp.StatusCode} {resp.StatusCode}");
                _output.WriteLine("[/] Body:");
                _output.WriteLine(body);
            }

            // Aceita 200/3xx/401/403/404 etc — só não pode ser erro de servidor
            Assert.DoesNotContain(resp.StatusCode, new[]
            {
                HttpStatusCode.InternalServerError,   // 500
                HttpStatusCode.BadGateway,            // 502
                HttpStatusCode.ServiceUnavailable     // 503
            });
        }
    }
}
