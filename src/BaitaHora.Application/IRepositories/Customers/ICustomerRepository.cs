using BaitaHora.Application.Features.Customers.Get.ReadModels;
using BaitaHora.Domain.Features.Schedules.Entities;

namespace BaitaHora.Application.IRepositories.Customers
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<IReadOnlyList<CustomerOptions>> SearchCustomersAsync(string search, CancellationToken ct = default);
    }
}