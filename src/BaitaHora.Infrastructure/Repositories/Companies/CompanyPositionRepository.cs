using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;

namespace BaitaHora.Infrastructure.Repositories.Companies
{
    public class CompanyPositionRepository : GenericRepository<CompanyPosition>, ICompanyPositionRepository
    {
        public CompanyPositionRepository(AppDbContext context) : base(context) { }
    }
}