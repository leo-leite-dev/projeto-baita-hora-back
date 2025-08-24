using FluentValidation;
using BaitaHora.Application.Features.Users.Commands;
using BaitaHora.Application.Features.Commons.Commands;

public sealed class PatchUserProfileCommandValidator : AbstractValidator<PatchUserProfileCommand>
{
    public PatchUserProfileCommandValidator()
    {
        // Nome: só valida se veio
        When(p => !string.IsNullOrWhiteSpace(p.FullName), () =>
        {
            RuleFor(p => p.FullName!)
                .MaximumLength(200)
                .WithMessage("Nome muito longo (máx 200).");
        });

        // BirthDate (DateOnly?): se vier, não pode ser futura
        When(p => p.BirthDate.HasValue, () =>
        {
            RuleFor(p => p.BirthDate!.Value)
                .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Data de nascimento não pode ser no futuro.");
        });

        // Telefone: só valida se vier
        When(p => !string.IsNullOrWhiteSpace(p.UserPhone), () =>
        {
            // Opção A: se tiver Phone.TryParse
            // RuleFor(p => p.UserPhone!)
            //    .Must(v => Phone.TryParse(v, out _))
            //    .WithMessage("Telefone inválido.");

            // Opção B: regex simples (mantém flexível)
            RuleFor(p => p.UserPhone!)
                .Matches(@"^\+?\d[\d\s\-()]{7,}$")
                .WithMessage("Telefone inválido.");
        });

        // CPF: só valida se vier
        When(p => !string.IsNullOrWhiteSpace(p.Cpf), () =>
        {
            // Opção A: VO
            // RuleFor(p => p.Cpf!)
            //     .Must(v => CPF.TryParse(v, out _))
            //     .WithMessage("CPF inválido.");

            // Opção B: regex (###.###.###-## ou 11 dígitos)
            RuleFor(p => p.Cpf!)
                .Matches(@"^\d{11}$|^\d{3}\.?\d{3}\.?\d{3}\-?\d{2}$")
                .WithMessage("CPF inválido.");
        });

        // RG: só valida se vier
        When(p => !string.IsNullOrWhiteSpace(p.Rg), () =>
        {
            // Opção A: VO
            // RuleFor(p => p.Rg!)
            //     .Must(v => RG.TryParse(v, out _))
            //     .WithMessage("RG inválido.");

            // Opção B: regra simples
            RuleFor(p => p.Rg!)
                .Matches(@"^[A-Za-z0-9.\-\/]{5,20}$")
                .WithMessage("RG inválido.");
        });

        // Address: opcional; se vier, valida campos presentes
        When(p => p.Address is not null, () =>
        {
            RuleFor(p => p.Address!).SetValidator(new PatchAddressCommandValidator());
        });
    }
}
