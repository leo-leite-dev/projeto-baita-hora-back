using BaitaHora.Domain.Features.Commons.Exceptions;

namespace BaitaHora.Domain.Companies.ValueObjects;

public readonly record struct CNPJ
{
    public string Value { get; }

    private CNPJ(string normalized) => Value = normalized;

    public static CNPJ Parse(string input)
    {
        if (!TryParse(input, out var doc))
            throw new UserException("Documento da empresa inválido.");
        return doc;
    }

    public static bool TryParse(string? input, out CNPJ cnpj)
    {
        cnpj = default;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var normalized = new string(input.Where(char.IsLetterOrDigit).ToArray());

        if (normalized.Length != 14)
            return false;

        cnpj = new CNPJ(normalized);
        return true;
    }

    public override string ToString() => Value;
}