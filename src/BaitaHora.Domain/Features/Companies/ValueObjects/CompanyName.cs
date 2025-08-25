using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Domain.Companies.ValueObjects;

public readonly record struct CompanyName
{
    public const int MinLength = 3;
    public const int MaxLength = 200;

    public string Value { get; }

    private CompanyName(string normalized) => Value = normalized;

    public static CompanyName Parse(string input)
    {
        if (!TryParse(input, out var name))
            throw new CompanyException("Nome da empresa inválido.");
        return name;
    }

    public static bool TryParse(string? input, out CompanyName name)
    {
        name = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var normalized = input.Trim();

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            return false;

        name = new CompanyName(normalized);
        return true;
    }

    public bool IsDefault => Value is null;
    public override string ToString() => Value ?? string.Empty;
}