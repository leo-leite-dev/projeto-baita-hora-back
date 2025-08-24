using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BaitaHora.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Http;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Application.Common.Errors; 
using Npgsql;                             

public class ExceptionHttpMappingMiddlewareTests
{
    private sealed class FakeDomainException : DomainException
    {
        public FakeDomainException(string message) : base(message) { }
    }

    private static TestServer BuildServer(ILoggerProvider? loggerProvider = null)
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging(lb =>
                {
                    if (loggerProvider is not null)
                        lb.AddProvider(loggerProvider);
                });

                services.AddSingleton<IDbErrorTranslator, FakeDbErrorTranslator>();
            })
            .Configure(app =>
            {
                app.UseMiddleware<ExceptionHttpMappingMiddleware>();

                app.Map("/throw-company", a => a.Run(ctx => throw new CompanyException("Empresa já existe.")));
                app.Map("/throw-user", a => a.Run(ctx => throw new UserException("Usuário inválido.")));
                app.Map("/throw-domain", a => a.Run(ctx => throw new FakeDomainException("Regra de domínio violada.")));
                app.Map("/throw-unknown", a => a.Run(ctx => throw new InvalidOperationException("boom")));
                app.Map("/ok", a => a.Run(async ctx => await ctx.Response.WriteAsync("ok")));

                app.Map("/started", a => a.Run(async ctx =>
                {
                    ctx.Response.ContentType = "text/plain; charset=utf-8";
                    await ctx.Response.WriteAsync("partial");
                    throw new CompanyException("Empresa já existe.");
                }));
            });

        return new TestServer(builder);
    }

    [Fact]
    public async Task CompanyException_DeveRetornar_409_ProblemDetails()
    {
        using var server = BuildServer();
        var client = server.CreateClient();

        var resp = await client.GetAsync("/throw-company");
        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
        resp.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var problem = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Detail.Should().Be("Empresa já existe.");
        problem.Title.Should().NotBeNullOrWhiteSpace();
        problem.Status.Should().Be((int)HttpStatusCode.Conflict);
        problem.Instance.Should().Be("/throw-company");
    }

    [Fact]
    public async Task ProblemDetails_DeveEstarEmCamelCaseEConterInstance()
    {
        using var server = BuildServer();
        var client = server.CreateClient();

        var resp = await client.GetAsync("/throw-company");
        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var raw = await resp.Content.ReadAsStringAsync();
        raw.Should().Contain("\"status\"");
        raw.Should().Contain("\"title\"");
        raw.Should().Contain("\"detail\"");
        raw.Should().Contain("\"instance\":\"/throw-company\"");
        raw.Should().NotContain("\"Status\"");
    }

    [Fact]
    public async Task UserException_DeveRetornar_409_ProblemDetails()
    {
        using var server = BuildServer();
        var client = server.CreateClient();

        var resp = await client.GetAsync("/throw-user");
        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
        resp.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var problem = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Detail.Should().Be("Usuário inválido.");
        problem.Instance.Should().Be("/throw-user");
    }

    [Fact]
    public async Task DomainException_Fallback_DeveRetornar_422()
    {
        using var server = BuildServer();
        var client = server.CreateClient();

        var resp = await client.GetAsync("/throw-domain");
        ((int)resp.StatusCode).Should().Be(422);
        resp.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var problem = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Title.Should().Contain("domínio");
        problem.Instance.Should().Be("/throw-domain");
    }

    [Fact]
    public async Task ExcecaoInesperada_DeveRetornar_500_Com_TraceId_E_LogError()
    {
        var loggerProvider = new TestLoggerProvider();
        using var server = BuildServer(loggerProvider);
        var client = server.CreateClient();

        var resp = await client.GetAsync("/throw-unknown");
        resp.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        resp.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var problem = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Title.Should().Contain("Erro");
        problem.Detail.Should().Contain("TraceId");

        loggerProvider.Entries.Should().Contain(e =>
            e.level == LogLevel.Error &&
            e.message.Contains("TraceId"));
    }

    [Fact]
    public async Task QuandoRespostaJaIniciada_NaoDeveSobrescreverProblemDetails()
    {
        using var server = BuildServer();
        var client = server.CreateClient();

        var resp = await client.GetAsync("/started");

        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("partial");
        resp.Content.Headers.ContentType!.MediaType.Should().NotBe("application/problem+json");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HappyPath_DeveRetornar_200()
    {
        using var server = BuildServer();
        var client = server.CreateClient();

        var resp = await client.GetAsync("/ok");
        resp.EnsureSuccessStatusCode();
        (await resp.Content.ReadAsStringAsync()).Should().Be("ok");
    }

    private sealed class TestLoggerProvider : ILoggerProvider
    {
        public readonly List<(LogLevel level, string message)> Entries = new();

        public ILogger CreateLogger(string categoryName) => new TestLogger(Entries);
        public void Dispose() { }

        private sealed class TestLogger : ILogger
        {
            private readonly List<(LogLevel level, string message)> _entries;
            public TestLogger(List<(LogLevel, string)> entries) => _entries = entries;

            public IDisposable BeginScope<TState>(TState state) => default!;
            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                                    Func<TState, Exception?, string> formatter)
            {
                var msg = formatter(state, exception);
                _entries.Add((logLevel, msg));
            }
        }
    }

    private sealed class FakeDbErrorTranslator : IDbErrorTranslator
    {
        public string? TryTranslateUniqueViolation(PostgresException ex)
            => $"Violação de unicidade ({ex.ConstraintName})";
    }
}