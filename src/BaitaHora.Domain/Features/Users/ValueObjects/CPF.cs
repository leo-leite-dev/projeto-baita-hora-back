using BaitaHora.Domain.Features.Common.Exceptions;
using Caelum.Stella.CSharp.Validation;

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
        if (s.Length != 11) return false;

        var isValid = new CPFValidator().IsValid(s);
        if (!isValid) return false;

        cpf = new CPF(s);
        return true;
    }

    public string Format()
        => Value.Length == 11
            ? $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}"
            : Value;

    public override string ToString() => Value;
}