using System.Text.RegularExpressions;
using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Domain.Features.Common.ValueObjects;

public readonly record struct Phone
{
    public string Value { get; }
    private Phone(string e164) => Value = e164;

    public static Phone Parse(string input) => Parse(input, "+55");

    public static Phone Parse(string input, string defaultCountryCode)
    {
        if (!TryParse(input, out var phone, defaultCountryCode))
            throw new UserException("Telefone inválido.");

        return phone;
    }

    public static bool TryParse(string? input, out Phone phone)
        => TryParse(input, out phone, "+55");

    public static bool TryParse(string? input, out Phone phone, string defaultCountryCode)
    {
        phone = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var trimmed = input.Trim();
        var digits = Regex.Replace(trimmed, @"\D", "");
        var ccDigits = Regex.Replace(defaultCountryCode ?? "", @"\D", "");

        if (string.IsNullOrEmpty(ccDigits))
            return false;

        var ccWithPlus = "+" + ccDigits;

        string e164;

        if (trimmed.StartsWith("+"))
            e164 = "+" + digits;

        else if (digits.StartsWith("00"))
            e164 = "+" + digits.Substring(2);

        else if (digits.StartsWith(ccDigits))
            e164 = "+" + digits;

        else
        {
            var local = digits.TrimStart('0');
            e164 = ccWithPlus + local;
        }

        if (!Regex.IsMatch(e164, @"^\+[1-9]\d{7,14}$"))
            return false;

        phone = new Phone(e164);
        return true;
    }

    public override string ToString() => Value;
    public static implicit operator string(Phone p) => p.Value;
}