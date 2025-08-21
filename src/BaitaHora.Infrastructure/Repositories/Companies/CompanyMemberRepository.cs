using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companie;

public sealed class CompanyMemberRepository : ICompanyMemberRepository
{
    private readonly AppDbContext _db;
    public CompanyMemberRepository(AppDbContext db) => _db = db;

    public async Task<CompanyMember?> GetAsync(Guid companyId, Guid userId, CancellationToken ct)
        => await _db.CompanyMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CompanyId == companyId && m.UserId == userId, ct);

    public async Task<(bool found, CompanyRole role, bool isActive)> GetRoleAsync(Guid companyId, Guid userId, CancellationToken ct)
    {
        var tuple = await _db.CompanyMembers
            .Where(m => m.CompanyId == companyId && m.UserId == userId)
            .Select(m => new { m.Role, m.IsActive })
            .FirstOrDefaultAsync(ct);

        return tuple is null
            ? (false, default, false)
            : (true, tuple.Role, tuple.IsActive);
    }
}