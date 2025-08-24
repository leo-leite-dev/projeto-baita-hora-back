using System.Text.RegularExpressions;
using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Domain.Features.Users.Validators;

public static class PhoneValidator
{
    private static readonly Regex _phoneRegex = new(
        @"^\+?(\d{1,3})?\s*(\(?\d{2}\)?\s*)?\d{4,5}-?\d{4}$",
        RegexOptions.Compiled
    );

    public static string Normalize(string phone)
    {
        if (phone is null) throw new UserException("Formato de telefone inválido.");

        var trimmed = phone.Trim();
        if (!_phoneRegex.IsMatch(trimmed))
            throw new UserException("Formato de telefone inválido.");

        return trimmed;
    }

    public static string? NormalizeOrNull(string? phone)
        => string.IsNullOrWhiteSpace(phone) ? null : Normalize(phone!);
}

//Telefone: só converta para VO se for assumir E.164 com lib robusta; caso contrário, mantenha seu validator atual.