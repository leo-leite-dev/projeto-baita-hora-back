using BaitaHora.Domain.Features.Common.Exceptions;

namespace BaitaHora.Domain.Companies.ValueObjects;

public readonly record struct CNPJ
{
    public const int Length = 14;

    public string Value { get; }

    private CNPJ(string normalizedDigits) => Value = normalizedDigits;

    public static CNPJ Parse(string input)
    {
        if (!TryParse(input, out var doc))
            throw new CompanyException("Documento da empresa inválido (CNPJ).");
        return doc;
    }

    public static bool TryParse(string? input, out CNPJ cnpj)
    {
        cnpj = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var digits = Clean(input);

        if (digits.Length != Length)
            return false;

        if (digits.All(d => d == digits[0]))
            return false;

        if (!HasValidCheckDigits(digits))
            return false;

        cnpj = new CNPJ(digits);
        return true;
    }

    public bool IsDefault => Value is null;

    public string ToFormattedString()
    {
        if (string.IsNullOrEmpty(Value) || Value.Length != Length)
            return string.Empty;

        return $"{Value[..2]}.{Value.Substring(2, 3)}.{Value.Substring(5, 3)}/{Value.Substring(8, 4)}-{Value.Substring(12, 2)}";
    }

    public override string ToString() => Value ?? string.Empty;

    private static string Clean(string input)
    {
        var span = input.AsSpan();
        var buffer = new System.Buffers.ArrayBufferWriter<char>(span.Length);

        for (int i = 0; i < span.Length; i++)
        {
            char c = span[i];
            if (char.IsDigit(c))
            {
                var dest = buffer.GetSpan(1); 
                dest[0] = c;                
                buffer.Advance(1);
            }
        }

        return buffer.WrittenSpan.ToString();
    }

    private static bool HasValidCheckDigits(string digits14)
    {
        var dv1 = CalcDv(digits14, useSecondDv: false);
        if (dv1 != digits14[12] - '0') return false;

        var dv2 = CalcDv(digits14, useSecondDv: true);
        return dv2 == digits14[13] - '0';
    }

    private static int CalcDv(string digits14, bool useSecondDv)
    {
        int[] w1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] w2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        if (!useSecondDv)
        {
            int sum = 0;
            for (int i = 0; i < 12; i++) sum += (digits14[i] - '0') * w1[i];
            var mod = sum % 11;
            return (mod < 2) ? 0 : 11 - mod;
        }
        else
        {
            int sum = 0;
            for (int i = 0; i < 13; i++) sum += (digits14[i] - '0') * w2[i];
            var mod = sum % 11;
            return (mod < 2) ? 0 : 11 - mod;
        }
    }
}