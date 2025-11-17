using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Feature.Customers;
using BaitaHora.Application.IRepositories.Customers;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Customers;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Application.Features.Customers.Create;

public sealed class CreateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerUseCase(ICustomerRepository customerRepository)
        => _customerRepository = customerRepository;

    public async Task<Result<Guid>> HandleAsync(CreateCustomerCommand cmd, CancellationToken ct)
    {
        var phone = Phone.Parse(cmd.CustomerPhone);
        var cpf   = CPF.Parse(cmd.CustomerCpf);

        var customer = Customer.Create(cmd.CustomerName, phone, cpf);

        await _customerRepository.AddAsync(customer, ct);

        return Result<Guid>.Created(customer.Id);
    }
}