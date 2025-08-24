using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Domain.Features.Companies.Enums;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companies
{
    public sealed class CompanyMemberRepository
        : GenericRepository<CompanyMember>, ICompanyMemberRepository
    {
        public CompanyMemberRepository(AppDbContext context) : base(context) { }

        public async Task<IReadOnlyList<CompanyMember>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Set<CompanyMember>()
                .Where(m => m.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task<CompanyMember?> GetByCompanyAndUserWithPositionAsync(
            Guid companyId, Guid userId, CancellationToken ct = default)
        {
            return await _context.Set<CompanyMember>()
                .AsNoTracking()
                .Include(m => m.PrimaryPosition) 
                .Include(m => m.Company)       
                .SingleOrDefaultAsync(
                    m => m.CompanyId == companyId && m.UserId == userId,
                    ct);
        }

        public async Task<(bool found, CompanyRole role, bool isActive)> GetRoleAsync(
            Guid companyId, Guid userId, CancellationToken ct)
        {
            var row = await _context.Set<CompanyMember>()
                .Where(m => m.CompanyId == companyId && m.UserId == userId)
                .Select(m => new { m.Role, m.IsActive })
                .FirstOrDefaultAsync(ct);

            return row is null
                ? (false, default, false)
                : (true, row.Role, row.IsActive);
        }
    }
}