using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Domain.Features.Users.ValueObjects;

public readonly record struct CPF
{
    public string Value { get; }

    private CPF(string normalized) => Value = normalized;

    public static CPF Parse(string input)
    {
        if (!TryParse(input, out var cpf))
            throw new UserException("CPF inválido.");
        return cpf;
    }

    public static bool TryParse(string? input, out CPF cpf)
    {
        cpf = default;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var s = new string(input.Where(char.IsDigit).ToArray());

        if (s.Length != 11)
            return false;

        if (s.Distinct().Count() == 1)
            return false;

        var a = s.Select(c => c - '0').ToArray();
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += a[i] * (10 - i);

        var d1 = sum % 11 < 2 ? 0 : 11 - (sum % 11);
        if (a[9] != d1) return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += a[i] * (11 - i);

        var d2 = sum % 11 < 2 ? 0 : 11 - (sum % 11);
        if (a[10] != d2) return false;

        cpf = new CPF(s);
        return true;
    }

    public override string ToString() => Value;
}