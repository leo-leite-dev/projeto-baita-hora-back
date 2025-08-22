using BaitaHora.Application.IRepositories;
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

        public async Task AddImageAsync(CompanyImage image, CancellationToken ct = default)
        {
            await _context.Set<CompanyImage>().AddAsync(image, ct);
        }
    }
}