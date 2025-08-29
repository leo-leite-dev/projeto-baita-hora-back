using BaitaHora.Domain.Features.Common;
using BaitaHora.Domain.Features.Common.Exceptions;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Domain.Features.Users.Entities;

public sealed class UserProfile : Entity
{
    public string FullName { get; private set; } = string.Empty;

    public CPF Cpf { get; private set; } = default!;
    public RG? Rg { get; private set; }

    public DateOfBirth? BirthDate { get; private set; }
    public Phone UserPhone { get; private set; }
    public Address Address { get; private set; } = default!;
    public string? ProfileImageUrl { get; private set; }

    private UserProfile() { }

    public static UserProfile Create(string fullName, CPF cpf, RG? rg, Phone userPhone, DateOfBirth? birthDate, Address address)
    {
        if (address is null) throw new UserException("Endereço é obrigatório.");

        {
            var profile = new UserProfile
            {
                FullName = fullName.Trim(),
                Cpf = cpf,
                Rg = rg,
                UserPhone = userPhone,
                BirthDate = birthDate,
                Address = address
            };

            profile.ValidateInvariants();
            return profile;
        }
    }

    private void ValidateInvariants()
    {
        if (string.IsNullOrWhiteSpace(FullName))
            throw new UserException("Nome completo é obrigatório.");

        if (FullName.Length > 120)
            throw new UserException("Nome completo deve ter no máximo 120 caracteres.");

        if (Cpf == default)
            throw new CompanyException("CPF é obrigatório.");

        if (UserPhone == default)
            throw new CompanyException("Telefone é obrigatório.");
    }

    public bool RenameFullName(string newFullName)
    {
        var value = (newFullName ?? string.Empty).Trim();
        if (value.Length < 3 || value.Length > 200)
            throw new UserException("O nome completo deve ter entre 3 e 200 caracteres.");

        if (string.Equals(FullName, value, StringComparison.Ordinal)) return false;
        FullName = value;
        return true;
    }

    public bool ChangeCpf(CPF newCpf)
    {
        if (Cpf.Equals(newCpf)) return false;
        Cpf = newCpf;
        return true;
    }

    public bool ChangeRg(RG? newRg)
    {
        if (Nullable.Equals(Rg, newRg)) return false;
        Rg = newRg;
        return true;
    }

    public bool ChangePhone(Phone newPhone)
    {
        if (UserPhone.Equals(newPhone)) return false;
        UserPhone = newPhone;
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
        if (newAddress is null) throw new UserException("Endereço é obrigatório.");
        if (Address is not null && Equals(Address, newAddress)) return false;

        Address = newAddress;
        return true;
    }

    public bool ChangeProfileImageUrl(string? url)
    {
        if (url is null) return false;
        var trimmed = url.Trim();
        string? newValue = null;

        if (trimmed.Length > 0)
        {
            if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new UserException("URL da imagem de perfil inválida.");

            newValue = trimmed;
        }

        if (string.Equals(ProfileImageUrl, newValue, StringComparison.Ordinal)) return false;

        ProfileImageUrl = newValue;
        return true;
    }
}