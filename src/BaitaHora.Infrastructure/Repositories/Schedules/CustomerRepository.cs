using BaitaHora.Application.Features.Customers.Get.ReadModels;
using BaitaHora.Application.IRepositories.Customers;
using BaitaHora.Domain.Features.Schedules.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Schedules;

public sealed class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<CustomerOptions>> SearchCustomersAsync(
        string search, CancellationToken ct = default)
    {
        return await _set
            .AsNoTracking()
            .Where(c => c.IsActive &&
                        (string.IsNullOrEmpty(search) ||
                         EF.Functions.ILike(c.Name, $"%{search}%")))
            .OrderBy(c => c.Name)
            .Select(c => new CustomerOptions(
                c.Id,
                c.Name,
                c.NoShowCount,
                c.NoShowPenaltyTotal
            ))
            .Take(15)
            .ToListAsync(ct);
    }
}