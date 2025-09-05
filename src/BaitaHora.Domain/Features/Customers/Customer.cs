using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Domain.Features.Customers;

public sealed class Customer : EntityBase
{
    public PersonName CustomerName { get; private set; }
    public Phone CustomerPhone { get; private set; }
    public CPF CustomerCpf { get; private set; }

    private Customer() { }

    public static Customer Create(PersonName name, Phone phone, CPF cpf)
    {
        var customer = new Customer()
        {
            CustomerName = name,
            CustomerPhone = phone,
            CustomerCpf = cpf
        };
        return customer;
    }
}