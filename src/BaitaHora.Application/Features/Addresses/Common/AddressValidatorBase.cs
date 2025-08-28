using System.Text.RegularExpressions;
using FluentValidation;

namespace BaitaHora.Application.Features.Addresses.Common;

public abstract class AddressValidatorBase<T> : AbstractValidator<T> where T : IAddressLike
{
    protected static readonly Regex CepPlain = new(@"^\d{8}$", RegexOptions.Compiled);
    protected static readonly Regex UfUpper2 = new(@"^[A-Z]{2}$", RegexOptions.Compiled);

    protected AddressValidatorBase(bool required)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        // Street
        When(x => required || x.Street is not null, () =>
        {
            var r = RuleFor(x => x.Street!);
            if (required) r.NotEmpty().WithMessage("A rua é obrigatória.");
            r.MinimumLength(3).WithMessage("A rua deve ter no mínimo 3 caracteres.")
             .MaximumLength(200).WithMessage("A rua deve ter no máximo 200 caracteres.");
        });

        // Number
        When(x => required || x.Number is not null, () =>
        {
            var r = RuleFor(x => x.Number!);
            if (required) r.NotEmpty().WithMessage("O número é obrigatório.");
            else r.NotEmpty().WithMessage("O número não pode ser vazio.");
            r.MaximumLength(20).WithMessage("O número deve ter no máximo 20 caracteres.");
        });

        // Complement
        When(x => required ? !string.IsNullOrWhiteSpace(x.Complement) : x.Complement is not null, () =>
        {
            RuleFor(x => x.Complement!)
                .MaximumLength(200).WithMessage("O complemento deve ter no máximo 200 caracteres.");
        });

        // Neighborhood
        When(x => required || x.Neighborhood is not null, () =>
        {
            var r = RuleFor(x => x.Neighborhood!);
            if (required) r.NotEmpty().WithMessage("O bairro é obrigatório.");
            else r.NotEmpty().WithMessage("O bairro não pode ser vazio.");
            r.MaximumLength(100).WithMessage("O bairro deve ter no máximo 100 caracteres.");
        });

        // City
        When(x => required || x.City is not null, () =>
        {
            var r = RuleFor(x => x.City!);
            if (required) r.NotEmpty().WithMessage("A cidade é obrigatória.");
            else r.NotEmpty().WithMessage("A cidade não pode ser vazia.");
            r.MaximumLength(100).WithMessage("A cidade deve ter no máximo 100 caracteres.");
        });

        // State
        When(x => required || x.State is not null, () =>
        {
            var r = RuleFor(x => x.State!);
            if (required) r.NotEmpty().WithMessage("A UF é obrigatória.");
            r.Length(2).WithMessage("A UF deve ter exatamente 2 caracteres.")
             .Must(s => s is not null && UfUpper2.IsMatch(s.Trim()))
                .WithMessage("A UF deve conter exatamente 2 letras maiúsculas (A-Z).");
        });

        // ZipCode
        When(x => required || x.ZipCode is not null, () =>
        {
            var r = RuleFor(x => x.ZipCode!);
            if (required) r.NotEmpty().WithMessage("O CEP é obrigatório.");
            r.Must(z => z is not null && CepPlain.IsMatch(z.Trim()))
                .WithMessage("CEP inválido. Use apenas 8 dígitos numéricos (sem traço).");
        });
    }
}