using System.Text.RegularExpressions;
using BaitaHora.Domain.Features.Commons.ValueObjects;
using FluentValidation;

namespace BaitaHora.Application.Features.Commons.Validators;

public static class CommonFluentValidators
{
    public static IRuleBuilderOptions<T, string> EmailVo<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .Must(e => Email.TryParse(e, out _))
                .WithMessage("E-mail inválido. Verifique se contém '@' e um domínio válido (ex: usuario@dominio.com).")
            .Must(NotStartOrEndWithDot)
                .WithMessage("O e-mail não pode começar ou terminar com ponto.")
            .Must(NoConsecutiveDots)
                .WithMessage("O e-mail não pode conter dois pontos consecutivos '..'.");
    }

    public static IRuleBuilderOptions<T, string?> OptionalEmailVo<T>(this IRuleBuilder<T, string?> rule)
    {
        return rule
            .Must(e => string.IsNullOrWhiteSpace(e) || Email.TryParse(e!, out _))
                .WithMessage("E-mail inválido. Verifique se contém '@' e um domínio válido (ex: usuario@dominio.com).")
            .Must(e => string.IsNullOrWhiteSpace(e) || NotStartOrEndWithDot(e))
                .WithMessage("O e-mail não pode começar ou terminar com ponto.")
            .Must(e => string.IsNullOrWhiteSpace(e) || NoConsecutiveDots(e))
                .WithMessage("O e-mail não pode conter dois pontos consecutivos '..'.");
    }

    private static bool NotStartOrEndWithDot(string? email)
        => !string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"(^\.)|(\.$)");

    private static bool NoConsecutiveDots(string? email)
        => !string.IsNullOrWhiteSpace(email) && !email.Contains("..");

    public static IRuleBuilderOptions<T, string> PhoneVo<T>(this IRuleBuilder<T, string> rule, string defaultCountryCode = "+55")
    {
        return rule
            .NotEmpty().WithMessage("O telefone é obrigatório.")
            .Must(p => Phone.TryParse(p, out _, defaultCountryCode))
                .WithMessage("Telefone inválido. Informe DDI/DDD e número. Ex.: +55 11 91234-5678 ou 11 91234-5678.")
            .Must(p => CountDigits(p) >= 8 && CountDigits(p) <= 15)
                .WithMessage("Telefone deve conter entre 8 e 15 dígitos (desconsiderando pontuação).");
    }

    public static IRuleBuilderOptions<T, string?> OptionalPhoneVo<T>(this IRuleBuilder<T, string?> rule, string defaultCountryCode = "+55")
    {
        return rule
            .Must(p => string.IsNullOrWhiteSpace(p) || Phone.TryParse(p!, out _, defaultCountryCode))
                .WithMessage("Telefone inválido. Informe DDI/DDD e número. Ex.: +55 11 91234-5678 ou 11 91234-5678.")
            .Must(p => string.IsNullOrWhiteSpace(p) || (CountDigits(p) >= 8 && CountDigits(p) <= 15))
                .WithMessage("Telefone deve conter entre 8 e 15 dígitos (desconsiderando pontuação).");
    }

    private static int CountDigits(string? s) => string.IsNullOrWhiteSpace(s) ? 0 : s.Count(char.IsDigit);
}