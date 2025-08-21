using BaitaHora.Domain.Commons.Exceptions;

namespace BaitaHora.Domain.Companies.ValueObjects;

public readonly record struct CompanyName
{
    public string Value { get; }

    private CompanyName(string normalized) => Value = normalized;

    public static CompanyName Parse(string input)
    {
        if (!TryParse(input, out var name))
            throw new UserException("Nome da empresa inválido.");
        return name;
    }

    public static bool TryParse(string? input, out CompanyName name)
    {
        name = default;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var normalized = input.Trim();
        if (normalized.Length < 3 || normalized.Length > 200)
            return false;

        name = new CompanyName(normalized);
        return true;
    }

    public override string ToString() => Value;
}