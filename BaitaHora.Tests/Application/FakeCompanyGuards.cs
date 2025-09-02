using System.Reflection;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Guards.Interfaces;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Tests.Application;

sealed class FakeCompanyGuards : ICompanyGuards
{
    private readonly AppDbContext _db;
    public FakeCompanyGuards(AppDbContext db) => _db = db;

    public async Task<Result<Company>> ExistsCompany(Guid companyId, CancellationToken ct)
    {
        var company = await _db.Set<Company>()
            .FirstOrDefaultAsync(c => c.Id == companyId, ct);

        if (company is not null)
            return Result<Company>.Ok(company);

        // cria stub usando ctor privado
        var stub = (Company)Activator.CreateInstance(typeof(Company), nonPublic: true)!;

        // seta o backing field do Id na classe base Entity
        var entityBaseType = typeof(BaitaHora.Domain.Features.Common.Entity);
        var idField = entityBaseType.GetField("<Id>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic);
        idField!.SetValue(stub, companyId);

        // ANEXA ao DbContext para que o EF rastreie
        _db.Attach(stub);
        _db.Entry(stub).State = EntityState.Unchanged;

        return Result<Company>.Ok(stub);
    }
}
