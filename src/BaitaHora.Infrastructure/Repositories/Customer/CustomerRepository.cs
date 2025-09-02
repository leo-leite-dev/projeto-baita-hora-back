using BaitaHora.Application.IRepositories.Customers;
using BaitaHora.Domain.Features.Customers;
using BaitaHora.Infrastructure.Data;

namespace BaitaHora.Infrastructure.Repositories.Customers
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context) { }
    }
}
