using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Domain.Features.Customers;

public sealed class Customer : Entity
{
    public Phone Phone { get; private set; }
    public CPF Cpf { get; private set; }

    private Customer() { }

    public static Customer Create(string name, Phone phone, CPF cpf)
    {
        var customer = new Customer()
        {
            Name = NormalizeAndValidateName(name),
            Phone = phone,
            Cpf = cpf
        };

        return customer;
    }
}