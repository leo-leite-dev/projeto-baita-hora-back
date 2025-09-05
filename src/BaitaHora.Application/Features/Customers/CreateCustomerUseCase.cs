using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Customers;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;
using BaitaHora.Domain.Features.Customers;
using BaitaHora.Application.Feature.Customers;

namespace BaitaHora.Application.Features.Customers.Create;

public sealed class CreateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerUseCase(ICustomerRepository customerRepository)
        => _customerRepository = customerRepository;

    public async Task<Result<CreateCustomerResponse>> HandleAsync(
        CreateCustomerCommand cmd, CancellationToken ct)
    {
        var name = PersonName.Parse(cmd.CustomerName);
        var phone = Phone.Parse(cmd.CustomerPhone);
        var cpf = CPF.Parse(cmd.CustomerCpf);

        var customer = Customer.Create(name, phone, cpf);
        await _customerRepository.AddAsync(customer, ct);

        var response = new CreateCustomerResponse(
            CustomerId: customer.Id,
            Name: name.Value,
            Phone: phone.Value,
            Cpf: cpf.Value
        );

        return Result<CreateCustomerResponse>.Created(response);
    }
}