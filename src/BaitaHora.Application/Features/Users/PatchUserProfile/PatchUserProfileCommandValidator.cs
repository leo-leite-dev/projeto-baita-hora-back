using BaitaHora.Application.Features.Addresses.PatchAddress;
using FluentValidation;

namespace BaitaHora.Application.Features.Users.PatchUserProfile;

public sealed class PatchUserProfileCommandValidator : AbstractValidator<PatchUserProfileCommand>
{
    public PatchUserProfileCommandValidator()
    {
        // Nome: só valida se veio
        When(p => !string.IsNullOrWhiteSpace(p.NewFullName), () =>
        {
            RuleFor(p => p.NewFullName!)
                .MaximumLength(200)
                .WithMessage("Nome muito longo (máx 200).");
        });

        // BirthDate (DateOnly?): se vier, não pode ser futura
        When(p => p.NewBirthDate.HasValue, () =>
        {
            RuleFor(p => p.NewBirthDate!.Value)
                .Must(d => d <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Data de nascimento não pode ser no futuro.");
        });

        // Telefone: só valida se vier
        When(p => !string.IsNullOrWhiteSpace(p.NewUserPhone), () =>
        {
            // Opção A: se tiver Phone.TryParse
            // RuleFor(p => p.Phone!)
            //    .Must(v => Phone.TryParse(v, out _))
            //    .WithMessage("Telefone inválido.");

            // Opção B: regex simples (mantém flexível)
            RuleFor(p => p.NewUserPhone!)
                .Matches(@"^\+?\d[\d\s\-()]{7,}$")
                .WithMessage("Telefone inválido.");
        });

        // CPF: só valida se vier
        When(p => !string.IsNullOrWhiteSpace(p.NewCpf), () =>
        {
            // Opção A: VO
            // RuleFor(p => p.Cpf!)
            //     .Must(v => CPF.TryParse(v, out _))
            //     .WithMessage("CPF inválido.");

            // Opção B: regex (###.###.###-## ou 11 dígitos)
            RuleFor(p => p.NewCpf!)
                .Matches(@"^\d{11}$|^\d{3}\.?\d{3}\.?\d{3}\-?\d{2}$")
                .WithMessage("CPF inválido.");
        });

        // RG: só valida se vier
        When(p => !string.IsNullOrWhiteSpace(p.NewRg), () =>
        {
            // Opção A: VO
            // RuleFor(p => p.Rg!)
            //     .Must(v => RG.TryParse(v, out _))
            //     .WithMessage("RG inválido.");

            // Opção B: regra simples
            RuleFor(p => p.NewRg!)
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
