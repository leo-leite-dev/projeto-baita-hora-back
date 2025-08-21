using System.Text.RegularExpressions;
using BaitaHora.Application.Addresses.DTOs.Inputs;
using FluentValidation;

namespace BaitaHora.Application.Address.Validators;

public sealed class AddressInputValidator : AbstractValidator<AddressInput>
{
    private static readonly Regex CepPlain = new(@"^\d{8}$", RegexOptions.Compiled);
    private static readonly Regex CepMasked = new(@"^\d{5}-\d{3}$", RegexOptions.Compiled);

    private static readonly HashSet<string> UfSet = new(StringComparer.OrdinalIgnoreCase)
    {
        "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG",
        "PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
    };

    public AddressInputValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("A rua é obrigatória.")
            .MaximumLength(120).WithMessage("A rua deve ter no máximo 120 caracteres.");

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("O número é obrigatório.")
            .MaximumLength(20).WithMessage("O número deve ter no máximo 20 caracteres.");

        When(x => !string.IsNullOrWhiteSpace(x.Complement), () =>
        {
            RuleFor(x => x.Complement!)
                .MaximumLength(120).WithMessage("O complemento deve ter no máximo 120 caracteres.");
        });

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage("O bairro é obrigatória.")
            .MaximumLength(80).WithMessage("O bairro deve ter no máximo 80 caracteres.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("A cidade é obrigatória.")
            .MaximumLength(80).WithMessage("A cidade deve ter no máximo 80 caracteres.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("A UF é obrigatória.")
            .Length(2).WithMessage("A UF deve ter exatamente 2 caracteres.")
            .Must(uf => uf is not null && UfSet.Contains(uf.Trim()))
                .WithMessage("UF inválida. Use uma sigla válida (ex.: SP, RJ, MG).");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("O CEP é obrigatório.")
            .Must(IsValidCep)
                .WithMessage("CEP inválido. Use 8 dígitos ou o formato 00000-000.");
    }

    private static bool IsValidCep(string? cep)
    {
        var v = (cep ?? string.Empty).Trim();
        return CepPlain.IsMatch(v) || CepMasked.IsMatch(v);
    }
}