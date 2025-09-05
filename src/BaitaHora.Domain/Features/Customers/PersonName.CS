using System.Text.RegularExpressions;

public readonly record struct PersonName
{
    public string Value { get; }
    private PersonName(string value) => Value = value;

    private static readonly Regex Pattern = new(
        @"^[\p{L}\p{M}][\p{L}\p{M} '\-]{1,48}[\p{L}\p{M}]$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static PersonName Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Nome obrigatório.");

        var norm = input.Trim();
        if (!Pattern.IsMatch(norm))
            throw new ArgumentException("Nome inválido. Deve ter entre 3 e 50 caracteres e aceitar acentos.");

        return new PersonName(norm);
    }

    public static bool TryParse(string? input, out PersonName result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var norm = input.Trim();
        if (!Pattern.IsMatch(norm)) return false;

        result = new PersonName(norm);
        return true;
    }

    public override string ToString() => Value;
    public static implicit operator string(PersonName n) => n.Value;
}