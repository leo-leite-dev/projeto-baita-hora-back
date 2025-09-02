using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using BaitaHora.Infrastructure.Data;
using BaitaHora.Application.Features.Companies.Catalog.Create;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Application.Common.Results;
using BaitaHora.Domain.Features.Companies.Entities;

// guard “bobo” que só devolve a company já anexada
file sealed class StubCompanyGuards(AppDbContext db) : ICompanyGuards
{
    public Task<Result<Company>> ExistsCompany(Guid companyId, CancellationToken ct)
    {
        var company = db.Set<Company>().Find(companyId)
                   ?? (Company)Activator.CreateInstance(typeof(Company), nonPublic: true)!;

        // seta o Id via backing field da Entity base
        var field = typeof(BaitaHora.Domain.Features.Common.Entity)
            .GetField("<Id>k__BackingField", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field!.SetValue(company, companyId);

        // garante que está “trackeada”
        db.Attach(company).State = EntityState.Unchanged;
        return Task.FromResult(Result<Company>.Ok(company));
    }
}

public class CreateCompanyServiceOffering_Smoke
{
    [Fact]
    public async Task CriaESalvaDiretoNoUseCase()
    {
        var companyId = Guid.Parse("0e1f2a08-abb0-4aa1-a378-851a599cb88b");

        await using var conn = new SqliteConnection("Data Source=:memory:");
        await conn.OpenAsync();

        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(conn));
        services.AddScoped<ICompanyGuards, StubCompanyGuards>();
        services.AddScoped<CreateCompanyServiceOfferingUseCase>();

        var sp = services.BuildServiceProvider();

        // schema
        using (var scope = sp.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();

            // (opcional) se quiser, persiste a Company “de verdade” aqui
            // db.Set<Company>().Add(companyReal); await db.SaveChangesAsync();
        }

        var cmd = new CreateCompanyServiceOfferingCommand
        {
            CompanyId = companyId,
            ServiceOfferingName = "Corte de Cabelo",
            Currency = "BRL",
            Amount = 79.90m
        };

        // chama o usecase DIRETO (sem MediatR/UnitOfWork)
        using var scope2 = sp.CreateScope();
        var usecase = scope2.ServiceProvider.GetRequiredService<CreateCompanyServiceOfferingUseCase>();
        var db2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();

        var res = await usecase.HandleAsync(cmd, CancellationToken.None);
        Assert.True(res.IsSuccess);

        // commit manual pra não depender do behavior
        await db2.SaveChangesAsync();

        var salvo = await db2.Set<CompanyServiceOffering>()
            .FirstOrDefaultAsync(x => x.Id == res.Value!.ServiceId);

        Assert.NotNull(salvo);
        Assert.Equal(companyId, salvo!.CompanyId);
        Assert.Equal("BRL", salvo.Price.Currency);
        Assert.Equal(79.90m, salvo.Price.Amount);
    }
}
