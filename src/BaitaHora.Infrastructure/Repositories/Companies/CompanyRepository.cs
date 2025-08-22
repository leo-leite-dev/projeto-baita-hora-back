using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Companies.ValueObjects;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companies
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(AppDbContext context) : base(context) { }

        public async Task<Company?> GetByIdWithMembersAndPositionsAsync(Guid companyId, CancellationToken ct)
        {
            return await _context.Companies
                .Where(c => c.Id == companyId)
                .Include(c => c.Members)
                .ThenInclude(m => m.PrimaryPosition)  
                .Include(c => c.Positions)
                .AsSplitQuery()
                .SingleOrDefaultAsync(ct);
        }

        public Task<bool> IsCompanyNameTakenAsync(CompanyName companyName, Guid? excludingCompanyId, CancellationToken ct)
        {
            var q = _context.Set<Company>()
                .AsNoTracking()
                .Where(c => c.CompanyName == companyName);

            if (excludingCompanyId.HasValue)
                q = q.Where(c => c.Id != excludingCompanyId.Value);

            return q.AnyAsync(ct);
        }

        public Task<bool> IsCnpjTakenAsync(CNPJ cnpj, Guid? excludingCompanyId, CancellationToken ct)
        {
            var q = _context.Set<Company>()
                .AsNoTracking()
                .Where(c => c.Cnpj == cnpj);

            if (excludingCompanyId.HasValue)
                q = q.Where(c => c.Id != excludingCompanyId.Value);

            return q.AnyAsync(ct);
        }

        public Task<bool> IsCompanyEmailTakenAsync(Email email, Guid? excludingCompanyId, CancellationToken ct)
        {
            var q = _context.Set<Company>()
                .AsNoTracking()
                .Where(c => c.CompanyEmail == email);

            if (excludingCompanyId.HasValue)
                q = q.Where(c => c.Id != excludingCompanyId.Value);

            return q.AnyAsync(ct);
        }

        public async Task AddImageAsync(CompanyImage image, CancellationToken ct = default)
        {
            await _context.Set<CompanyImage>().AddAsync(image, ct);
        }
    }
}