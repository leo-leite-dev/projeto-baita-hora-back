using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;                 // <-- novo
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Xunit;

using BaitaHora.Infrastructure.Data;
using BaitaHora.Application.Features.Companies.Catalog.Create;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Tests.Application;           // FakeCompanyGuards (teu arquivo) :contentReference[oaicite:1]{index=1}

public class CreateCompanyServiceOffering_AppTests
{
    [Fact]
    public async Task Deve_Criar_ServiceOffering_E_Persistir()
    {
        var companyId = Guid.Parse("0e1f2a08-abb0-4aa1-a378-851a599cb88b");

        // 1) Conexão SQLite em memória precisa ficar aberta até o fim do teste
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var services = new ServiceCollection();
        services.AddLogging();

        // 2) Usa SQLite In-Memory (relacional)
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(connection));

        // 3) Guards: usa o FAKE pra evitar dependência de repositórios/infra
        services.AddScoped<ICompanyGuards, FakeCompanyGuards>(); // :contentReference[oaicite:2]{index=2}

        // 4) MediatR + UseCase/Handler
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateCompanyServiceOfferingHandler>();
        });
        services.AddScoped<CreateCompanyServiceOfferingUseCase>();

        var sp = services.BuildServiceProvider();

        // 5) Cria schema no SQLite in-memory
        using (var scope = sp.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        var cmd = new CreateCompanyServiceOfferingCommand
        {
            CompanyId = companyId,
            ServiceOfferingName = "Corte de Cabelo",
            Currency = "BRL",
            Amount = 79.90m
        };

        using var scope2 = sp.CreateScope();
        var mediator = scope2.ServiceProvider.GetRequiredService<ISender>();

        var res = await mediator.Send(cmd, CancellationToken.None);

        Assert.True(res.IsSuccess);
        Assert.NotNull(res.Value);
        Assert.Equal("Corte de Cabelo", res.Value!.ServiceOfferingName);
        Assert.Equal("BRL", res.Value.Currency);
        Assert.Equal(79.90m, res.Value.Amount);

        var db2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();

        var salvo = await db2.Set<CompanyServiceOffering>() // consulta relacional OK
            .FirstOrDefaultAsync(x => x.Id == res.Value!.ServiceId);

        Assert.NotNull(salvo);
        Assert.Equal(companyId, salvo!.CompanyId);
    }
}
