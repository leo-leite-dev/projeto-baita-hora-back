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

        public Task<bool> HasAnyMembershipAsync(Guid userId, CancellationToken ct)
            => _set.AnyAsync(m => m.UserId == userId, ct);

        public Task<bool> IsMemberOfCompanyAsync(Guid companyId, Guid userId, CancellationToken ct)
            => _set.AnyAsync(m => m.CompanyId == companyId && m.UserId == userId, ct);

        public Task<CompanyMember?> GetMemberAsync(Guid companyId, Guid userId, CancellationToken ct)
            => _set.FirstOrDefaultAsync(m => m.CompanyId == companyId && m.UserId == userId, ct);

        public async Task<IReadOnlyList<CompanyMember>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
            => await _set.Where(m => m.UserId == userId).ToListAsync(ct);

        public async Task<CompanyMember?> GetByCompanyAndUserWithPositionAsync(
            Guid companyId, Guid userId, CancellationToken ct = default)
        {
            return await _set
                .AsNoTracking()
                .Include(m => m.PrimaryPosition)
                .Include(m => m.Company)
                .SingleOrDefaultAsync(m => m.CompanyId == companyId && m.UserId == userId, ct);
        }

        public async Task<(bool found, CompanyRole role, bool isActive)> GetRoleAsync(
            Guid companyId, Guid userId, CancellationToken ct)
        {
            var row = await _set
                .Where(m => m.CompanyId == companyId && m.UserId == userId)
                .Select(m => new { m.Role, m.IsActive })
                .FirstOrDefaultAsync(ct);

            return row is null
                ? (false, default, false)
                : (true, row.Role, row.IsActive);
        }
    }
}