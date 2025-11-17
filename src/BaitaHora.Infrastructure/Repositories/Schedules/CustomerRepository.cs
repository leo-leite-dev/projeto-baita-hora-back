using BaitaHora.Application.Features.Customers.Get.ReadModels;
using BaitaHora.Application.IRepositories.Customers;
using BaitaHora.Domain.Features.Customers;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Customers;

public sealed class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public Task<IReadOnlyList<CustomerOptions>> SearchCustomersAsync(string search, CancellationToken ct = default)
        => _set.AsNoTracking()
               .Where(c => c.IsActive &&
                           (string.IsNullOrEmpty(search) ||
                            EF.Functions.ILike(c.Name, $"%{search}%")))
               .OrderBy(c => c.Name)
               .Select(c => new CustomerOptions(c.Id, c.Name))
               .Take(15)
               .ToListAsync(ct)
               .ContinueWith<IReadOnlyList<CustomerOptions>>(t => t.Result, ct);
}