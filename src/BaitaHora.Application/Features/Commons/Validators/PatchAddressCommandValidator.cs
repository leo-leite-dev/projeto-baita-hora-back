using System.Text.RegularExpressions;
using BaitaHora.Application.Features.Commons.Commands;
using FluentValidation;

public sealed class PatchAddressCommandValidator : AbstractValidator<PatchAddressCommand>
{
    private static readonly Regex CepPlain = new(@"^\d{8}$", RegexOptions.Compiled);
    private static readonly Regex UfUpper2 = new(@"^[A-Z]{2}$", RegexOptions.Compiled);

    private static readonly HashSet<string> UfSet = new(StringComparer.Ordinal)
    {
        "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG",
        "PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
    };

    public PatchAddressCommandValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        When(x => !string.IsNullOrWhiteSpace(x.Street), () =>
        {
            RuleFor(x => x.Street!)
                .MinimumLength(3).WithMessage("A rua deve ter no mínimo 3 caracteres.")
                .MaximumLength(200).WithMessage("A rua deve ter no máximo 200 caracteres.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Number), () =>
        {
            RuleFor(x => x.Number!)
                .MaximumLength(20).WithMessage("O número deve ter no máximo 20 caracteres.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Complement), () =>
        {
            RuleFor(x => x.Complement!)
                .MaximumLength(200).WithMessage("O complemento deve ter no máximo 200 caracteres.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.Neighborhood), () =>
        {
            RuleFor(x => x.Neighborhood!)
                .MaximumLength(100).WithMessage("O bairro deve ter no máximo 100 caracteres.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.City), () =>
        {
            RuleFor(x => x.City!)
                .MaximumLength(100).WithMessage("A cidade deve ter no máximo 100 caracteres.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.State), () =>
        {
            RuleFor(x => x.State!)
                .Length(2).WithMessage("A UF deve ter exatamente 2 caracteres.")
                .Must(s => UfUpper2.IsMatch(s))
                    .WithMessage("A UF deve conter exatamente 2 letras maiúsculas (A-Z).")
                .Must(s => UfSet.Contains(s))
                    .WithMessage("UF inválida. Use uma sigla válida (ex.: SP, RJ, MG).");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ZipCode), () =>
        {
            RuleFor(x => x.ZipCode!)
                .Must(z => CepPlain.IsMatch(z))
                    .WithMessage("CEP inválido. Use apenas 8 dígitos numéricos (sem traço).");
        });
    }
}