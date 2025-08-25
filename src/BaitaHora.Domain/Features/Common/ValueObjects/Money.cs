using System.Globalization;

namespace BaitaHora.Domain.Common.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Currency = NormalizeCurrency(currency); 
        Amount = Math.Round(amount, 2, MidpointRounding.ToEven);
    }

    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0m) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-negative.");
        return new Money(amount, currency);
    }

    public override string ToString()
        => $"{Currency} {Amount.ToString("F2", CultureInfo.InvariantCulture)}";

    private static string NormalizeCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter ISO-4217 code.", nameof(currency));
        return currency.ToUpperInvariant();
    }
}
