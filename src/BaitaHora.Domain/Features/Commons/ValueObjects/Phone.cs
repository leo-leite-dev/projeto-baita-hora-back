using System.Text.RegularExpressions;
using BaitaHora.Domain.Features.Commons.Exceptions;

namespace BaitaHora.Domain.Features.Commons.ValueObjects;

public readonly record struct Phone
{
    public string Value { get; }
    private Phone(string normalizedE164) => Value = normalizedE164;

    public static Phone Parse(string input)
        => Parse(input, "+55");

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
        if (string.IsNullOrWhiteSpace(input)) return false;

        var hasPlusPrefix = input.TrimStart().StartsWith("+");
        var digits = Regex.Replace(input, @"\D", "");
        var cc = NormalizeCountryCode(defaultCountryCode);

        var e164 = hasPlusPrefix ? "+" + digits : cc + digits;

        if (!Regex.IsMatch(e164, @"^\+[1-9]\d{7,14}$")) return false;

        phone = new Phone(e164);
        return true;

        static string NormalizeCountryCode(string cc)
        {
            var d = Regex.Replace(cc ?? string.Empty, @"\D", ""); 
            return "+" + d;
        }
    }

    public override string ToString() => Value;
}