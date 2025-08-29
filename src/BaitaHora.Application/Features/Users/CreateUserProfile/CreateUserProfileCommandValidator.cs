using System.Text.RegularExpressions;
using BaitaHora.Application.Features.Addresses.Create;
using BaitaHora.Domain.Features.Common.ValueObjects;
using BaitaHora.Domain.Features.Users.ValueObjects;
using FluentValidation;

namespace BaitaHora.Application.Features.Users.CreateUserProfile;

public sealed class CreateUserProfileCommandValidator : AbstractValidator<CreateUserProfileCommand>
{
    public CreateUserProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("O nome completo é obrigatório.")
            .MinimumLength(3).WithMessage("O nome completo deve ter pelo menos 3 caracteres.")
            .MaximumLength(200).WithMessage("O nome completo deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("O CPF é obrigatório.")
            .Must(c => CPF.TryParse(c, out _))
            .WithMessage("CPF inválido. Informe no formato 00000000000 ou 000.000.000-00.");

        When(x => !string.IsNullOrWhiteSpace(x.Rg), () =>
        {
            RuleFor(x => x.Rg!)
                .Must(r => NormalizedLen(r) >= 5)
                    .WithMessage("RG muito curto. Deve ter pelo menos 5 caracteres (desconsiderando pontos e traços).")
                .Must(r => NormalizedLen(r) <= 20)
                    .WithMessage("RG muito longo. Deve ter no máximo 20 caracteres (desconsiderando pontos e traços).")
                .Must(r => RG.TryParse(r, out _))
                    .WithMessage("RG inválido. Use apenas letras e números (pontos e traços serão ignorados).");
        });

        RuleFor(x => x.UserPhone)
            .NotEmpty().WithMessage("O telefone é obrigatório.")
            .Must(p => Phone.TryParse(p, out _))
                .WithMessage("Telefone inválido. Informe DDD e número. Exemplos: 11 91234-5678 ou +55 11 91234-5678.");

        RuleFor(x => x.UserPhone)
            .Must(p => Digits(p) >= 8 && Digits(p) <= 15)
                .WithMessage("Telefone deve conter entre 8 e 15 dígitos (desconsiderando pontuação).")
            .When(x => !string.IsNullOrWhiteSpace(x.UserPhone));

        RuleFor(x => x.BirthDate)
            .Must(d => d == null || d.Value <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("A data de nascimento não pode estar no futuro.")
            .Must(d => d == null || d.Value >= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-120)))
                .WithMessage("Data de nascimento muito antiga.")
            .Must(IsAdult)
                .WithMessage("Usuário deve ter pelo menos 18 anos.")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.Address)
            .NotNull().WithMessage("O endereço é obrigatório.")
            .SetValidator(new CreateAddressCommandValidator());
    }

    private static int NormalizedLen(string? rg)
    {
        if (string.IsNullOrWhiteSpace(rg)) return 0;
        var normalized = Regex.Replace(rg, @"[^0-9A-Za-z]", string.Empty);
        return normalized.Length;
    }

    private static int Digits(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return 0;
        return Regex.Replace(s, @"\D", string.Empty).Length;
    }

    private static bool IsAdult(DateOnly? birthDate)
    {
        if (!birthDate.HasValue) return true;

        var birth = birthDate.Value;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var age = today.Year - birth.Year;
        if (birth > today.AddYears(-age)) age--;

        return age >= 18;
    }
}