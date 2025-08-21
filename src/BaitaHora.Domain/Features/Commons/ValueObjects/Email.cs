using System.Net.Mail;
using BaitaHora.Domain.Features.Commons.Exceptions;

namespace BaitaHora.Domain.Features.Commons.ValueObjects;

public readonly record struct Email
{
    public string Value { get; }

    private Email(string normalized) => Value = normalized;

    public static Email Parse(string input)
    {
        if (!TryParse(input, out var email))
            throw new UserException("E-mail inválido.");
        return email;
    }

    public static bool TryParse(string? input, out Email email)
    {
        email = default;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var normalized = input.Trim().ToLowerInvariant();
        try
        {
            _ = new MailAddress(normalized);
            email = new Email(normalized);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => Value;
}