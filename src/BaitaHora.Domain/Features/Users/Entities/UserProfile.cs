using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Domain.Features.Users.Entities;

public sealed class UserProfile : Entity
{
    public CPF Cpf { get; private set; } = default!;
    public RG? Rg { get; private set; }

    public DateOfBirth? BirthDate { get; private set; }
    public Phone Phone { get; private set; }
    public Address Address { get; private set; } = default!;
    public string? ProfileImageUrl { get; private set; }

    private UserProfile() { }

    public static UserProfile Create(string name, CPF cpf, RG? rg, Phone Phone, DateOfBirth? birthDate, Address address)
    {
        if (address is null)
            throw new UserException("Endereço é obrigatório.");

        {
            var profile = new UserProfile
            {
                Name = NormalizeAndValidateName(name),
                Cpf = cpf,
                Rg = rg,
                Phone = Phone,
                BirthDate = birthDate,
                Address = address
            };

            profile.ValidateInvariants();
            return profile;
        }
    }

    private void ValidateInvariants()
    {
        if (Cpf == default)
            throw new CompanyException("CPF é obrigatório.");

        if (Phone == default)
            throw new CompanyException("Telefone é obrigatório.");
    }

    public bool Rename(string newName)
    {
        var normalized = NormalizeAndValidateName(newName);

        if (NameEquals(Name, normalized))
            return false;

        Name = normalized;
        Touch();
        return true;
    }

    public bool ChangeCpf(CPF newCpf)
    {
        if (Cpf.Equals(newCpf))
            return false;

        Cpf = newCpf;
        return true;
    }

    public bool ChangeRg(RG? newRg)
    {
        if (Nullable.Equals(Rg, newRg))
            return false;

        Rg = newRg;
        Touch();
        return true;
    }

    public bool ChangePhone(Phone newPhone)
    {
        if (Phone.Equals(newPhone))
            return false;

        Phone = newPhone;
        Touch();
        return true;
    }

    public bool ChangeBirthDate(DateOfBirth? newDob)
    {
        if (BirthDate.HasValue == newDob.HasValue &&
            (!BirthDate.HasValue || BirthDate!.Value.Equals(newDob!.Value)))
            return false;

        BirthDate = newDob;
        Touch();
        return true;
    }

    public bool ChangeAddress(Address newAddress)
    {
        if (newAddress is null)
            throw new UserException("Endereço é obrigatório.");

        if (Address is not null && Equals(Address, newAddress))
            return false;

        Address = newAddress;
        Touch();
        return true;
    }
}