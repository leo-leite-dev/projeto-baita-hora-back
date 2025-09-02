using BaitaHora.Application.Feature.Customers;
using BaitaHora.Contracts.DTOs.Customers;

namespace BaitaHora.Api.Mappers.Customers;

public static class CustomersApiMappers
{
    public static CreateCustomerCommand ToCommand(
        this CreateCustomerRequest r, Guid companyId)
        => new CreateCustomerCommand
        {
            CompanyId = companyId,
            CustomerName = r.CustomerName,
            CustomerPhone = r.CustomerPhone,
            CustomerCpf = r.CustomerCpf
        };
}