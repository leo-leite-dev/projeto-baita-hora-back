using BaitaHora.Domain.Features.Commons.Exceptions;

namespace BaitaHora.Domain.Features.Users.ValueObjects;

public readonly record struct RG
{
    public string Value { get; }

    private RG(string normalized) => Value = normalized;

    public static RG Parse(string input)
    {
        if (!TryParse(input, out var rg))
            throw new UserException("RG inválido.");
        return rg;
    }

    public static bool TryParse(string? input, out RG rg)
    {
        rg = default;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var normalized = new string(input.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

        if (normalized.Length < 5) return false;

        rg = new RG(normalized);
        return true;
    }

    public override string ToString() => Value;
}