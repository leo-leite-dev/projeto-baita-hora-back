using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;

namespace BaitaHora.Infrastructure.Repositories.Companies
{
    public class CompanyServiceOfferingRepository : GenericRepository<CompanyServiceOffering>, ICompanyServiceOfferingRepository
    {
        public CompanyServiceOfferingRepository(AppDbContext context) : base(context) { }
    }
}