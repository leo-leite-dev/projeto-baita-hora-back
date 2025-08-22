using System.Text.RegularExpressions;
using BaitaHora.Application.Features.Commons.Commands;
using FluentValidation;

namespace BaitaHora.Application.Features.Commons.Validators;

public sealed class AddressCommandValidator : AbstractValidator<AddressCommand>
{
    private static readonly Regex CepPlain = new(@"^\d{8}$", RegexOptions.Compiled);
    private static readonly Regex UfUpper2 = new(@"^[A-Z]{2}$", RegexOptions.Compiled);

    private static readonly HashSet<string> UfSet = new(StringComparer.Ordinal)  
    {
        "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG",
        "PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
    };

    public AddressCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("A rua é obrigatória.")
            .MinimumLength(3).WithMessage("A rua deve ter no mínimo 3 caracteres.")
            .MaximumLength(200).WithMessage("A rua deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("O número é obrigatório.")
            .MaximumLength(20).WithMessage("O número deve ter no máximo 20 caracteres.");

        When(x => !string.IsNullOrWhiteSpace(x.Complement), () =>
        {
            RuleFor(x => x.Complement!)
                .MaximumLength(200).WithMessage("O complemento deve ter no máximo 200 caracteres.");
        });

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage("O bairro é obrigatório.")
            .MaximumLength(100).WithMessage("O bairro deve ter no máximo 100 caracteres.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("A cidade é obrigatória.")
            .MaximumLength(100).WithMessage("A cidade deve ter no máximo 100 caracteres.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("A UF é obrigatória.")
            .Length(2).WithMessage("A UF deve ter exatamente 2 caracteres.")
            .Must(s => s is not null && UfUpper2.IsMatch(s.Trim()))
                .WithMessage("A UF deve conter exatamente 2 letras maiúsculas (A-Z).")
            .Must(s => s is not null && UfSet.Contains(s.Trim()))
                .WithMessage("UF inválida. Use uma sigla válida (ex.: SP, RJ, MG).");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("O CEP é obrigatório.")
            .Must(z => z is not null && CepPlain.IsMatch(z.Trim()))
                .WithMessage("CEP inválido. Use apenas 8 dígitos numéricos (sem traço).");
    }
}