using System.Globalization;

namespace BaitaHora.Domain.Common.ValueObjects;

public readonly record struct Money : IComparable<Money>
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

    public static Money RequirePositive(decimal amount, string currency)
    {
        if (amount <= 0m) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        return new Money(amount, currency);
    }

    public static Money operator +(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        var result = a.Amount - b.Amount;
        if (result < 0m) throw new InvalidOperationException("Resulting amount cannot be negative.");
        return new(result, a.Currency);
    }

    public int CompareTo(Money other)
    {
        EnsureSameCurrency(this, other);
        return Amount.CompareTo(other.Amount);
    }

    public override string ToString()
        => $"{Currency} {Amount.ToString("F2", CultureInfo.InvariantCulture)}";

    private static string NormalizeCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter ISO-4217 code.", nameof(currency));
        return currency.ToUpperInvariant();
    }

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (!string.Equals(a.Currency, b.Currency, StringComparison.Ordinal))
            throw new InvalidOperationException("Currency mismatch.");
    }
}