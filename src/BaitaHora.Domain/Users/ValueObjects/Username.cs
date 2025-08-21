using BaitaHora.Domain.Commons.Exceptions;
using System.Text.RegularExpressions;

namespace BaitaHora.Domain.Users.ValueObjects;

public readonly record struct Username
{
    public string Value { get; }

    private Username(string normalized) => Value = normalized;

    private static readonly Regex ValidPattern = new(@"^[a-zA-Z0-9._-]+$", RegexOptions.Compiled);

    public static Username Parse(string input)
    {
        if (!TryParse(input, out var username))
            throw new UserException("Username inválido.");
        return username;
    }

    public static bool TryParse(string? input, out Username username)
    {
        username = default;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var norm = input.Trim();

        if (norm.Length < 3 || norm.Length > 50) return false;
        if (!ValidPattern.IsMatch(norm)) return false;

        if (".-_".Contains(norm[0]) || ".-_".Contains(norm[^1])) return false;
        if (norm.Contains("..") || norm.Contains("__") || norm.Contains("--")) return false;

        username = new Username(norm);
        return true;
    }

    public override string ToString() => Value;
}