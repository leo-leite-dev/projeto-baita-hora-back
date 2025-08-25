using System.Text.RegularExpressions;
using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Common.ValueObjects;

namespace BaitaHora.Domain.Features.Customers;

public sealed class Customer : Entity
{
    public Username CustomerName { get; private set; }
    public Phone CustomerPhone { get; private set; }

    public bool IsActive { get; private set; } = true;

    private static readonly Regex E164 = new(@"^\+[1-9]\d{7,14}$", RegexOptions.Compiled);

    private Customer() { }

    public static Customer Create(Username name, Phone customerPhone)
    {
        var costumer = new Customer();
        costumer.SetName(name);
        costumer.SetPhone(customerPhone);
        costumer.IsActive = true;

        return costumer;
    }


    public bool SetName(Username newName)
    {
        if (string.IsNullOrWhiteSpace(newName.Value))
            throw new CustomerException("Nome do cliente é obrigatório.");

        if (CustomerName.Equals(newName)) return false;

        CustomerName = newName;

        return true;
    }

    public bool SetPhone(Phone newPhone)
    {
        if (string.IsNullOrEmpty(newPhone.Value))
            throw new CustomerException("Telefone inválido.");

        if (CustomerPhone.Equals(newPhone)) return false;

        CustomerPhone = newPhone;

        return true;
    }

    public bool SetActive(bool active)
    {
        if (IsActive == active) return false;
        IsActive = active;

        return true;
    }

    public void Activate() => SetActive(true);
    public void Deactivate() => SetActive(false);
}