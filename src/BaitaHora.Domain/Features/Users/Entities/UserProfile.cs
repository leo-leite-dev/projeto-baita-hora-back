using BaitaHora.Domain.Features.Commons;
using BaitaHora.Domain.Features.Commons.Exceptions;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;

namespace BaitaHora.Domain.Features.Users.Entities;

public sealed class UserProfile : Entity
{
    public string FullName { get; private set; } = string.Empty;

    public CPF Cpf { get; private set; } = default!;
    public RG? Rg { get; private set; }

    public DateTime? BirthDate { get; private set; }
    public Phone UserPhone { get; private set; }
    public Address Address { get; private set; } = default!;
    public string? ProfileImageUrl { get; private set; }

    private UserProfile() { }

    public static UserProfile Create(string fullName, CPF cpf, Phone UserPhone, Address address)
    {
        if (address is null) throw new UserException("Endereço é obrigatório.");

        var profile = new UserProfile();
        profile.SetAddress(address);
        profile.SetFullName(fullName);
        profile.SetCpf(cpf);
        profile.SetPhone(UserPhone);

        return profile;
    }

    public bool SetFullName(string newFullName)
    {
        var value = (newFullName ?? string.Empty).Trim();
        if (value.Length < 3 || value.Length > 200)
            throw new UserException("O nome completo deve ter entre 3 e 200 caracteres.");

        if (string.Equals(FullName, value, StringComparison.Ordinal)) return false;
        FullName = value;
        return true;
    }

    public bool SetCpf(CPF newCpf)
    {
        if (Cpf.Equals(newCpf)) return false;
        Cpf = newCpf;
        return true;
    }

    public bool SetRg(RG? newRg)
    {
        if (Nullable.Equals(Rg, newRg)) return false;
        Rg = newRg;
        return true;
    }

    public bool SetBirthDate(DateTime? newBirthDate)
    {
        DateTime? normalized = null;
        if (newBirthDate.HasValue)
        {
            var birth = newBirthDate.Value.Date;
            var today = DateTime.UtcNow.Date;

            if (birth > today)
                throw new UserException("A data de nascimento não pode estar no futuro.");
            if (birth < today.AddYears(-120))
                throw new UserException("Data de nascimento muito antiga.");

            var age = today.Year - birth.Year - (birth > today.AddYears(-(today.Year - birth.Year)) ? 1 : 0);
            if (age < 18)
                throw new UserException("Usuário deve ter pelo menos 18 anos.");

            normalized = birth;
        }

        if (BirthDate.HasValue == normalized.HasValue &&
            (!BirthDate.HasValue || BirthDate.Value.Date == normalized!.Value.Date))
            return false;

        BirthDate = normalized;
        return true;
    }

    public bool SetPhone(Phone newPhone)
    {
        if (UserPhone.Equals(newPhone)) return false;
        UserPhone = newPhone;
        return true;
    }

    public bool SetAddress(Address newAddress)
    {
        if (newAddress is null) throw new UserException("Endereço é obrigatório.");
        if (Address is not null && Equals(Address, newAddress)) return false;

        Address = newAddress;
        return true;
    }

    public bool SetProfileImageUrl(string? url)
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