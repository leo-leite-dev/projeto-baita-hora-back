using BaitaHora.Application.Feature.Schedules.Customers.Create;
using BaitaHora.Contracts.DTOs.Schedules.Customers;

namespace BaitaHora.Api.Mappers.Schedules.Customers;

public static class CustomersApiMappers
{
    public static CreateCustomerCommand ToCommand(
        this CreateCustomerRequest r)
        => new CreateCustomerCommand
        {
            CustomerName = r.CustomerName,
            CustomerPhone = r.CustomerPhone,
            CustomerCpf = r.CustomerCpf
        };
}