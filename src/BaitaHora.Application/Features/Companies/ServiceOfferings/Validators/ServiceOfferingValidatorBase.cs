using FluentValidation;
using System.Text.RegularExpressions;

namespace BaitaHora.Application.Features.Companies.ServiceOffering.Validators
{
    public abstract class ServiceOfferingValidatorBase<T> : AbstractValidator<T>
    {
        protected static readonly Regex ServiceNameRegex =
            new(@"^[\p{L}0-9 .,'\-&()/]+$", RegexOptions.Compiled);

        protected ServiceOfferingValidatorBase(
            bool required,
            Func<T, Guid?> companyIdSelector,
            Func<T, string?> nameSelector,
            Func<T, decimal?> amountSelector,
            Func<T, string?> currencySelector,
            Func<T, IEnumerable<Guid>?> positionIdsSelector = null!,
            Func<T, Guid?> serviceOfferingIdSelector = null!
        )
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => companyIdSelector(x))
                .NotNull().WithMessage("CompanyId é obrigatório.")
                .Must(id => id.HasValue && id.Value != Guid.Empty)
                .WithMessage("CompanyId inválido.");

            if (serviceOfferingIdSelector is not null)
            {
                RuleFor(x => serviceOfferingIdSelector(x))
                    .NotNull().WithMessage("ServiceOfferingId é obrigatório.")
                    .Must(id => id.HasValue && id.Value != Guid.Empty)
                    .WithMessage("ServiceOfferingId inválido.");
            }

            When(x => required || !string.IsNullOrWhiteSpace(nameSelector(x)), () =>
            {
                var r = RuleFor(x => nameSelector(x)!);

                if (required)
                    r.NotEmpty().WithMessage("ServiceOfferingName é obrigatório.");
                else
                    r.NotEmpty().WithMessage("ServiceOfferingName não pode ser vazio.");

                r.MinimumLength(3).WithMessage("ServiceOfferingName deve ter no mínimo 3 caracteres.")
                 .MaximumLength(120).WithMessage("ServiceOfferingName deve ter no máximo 120 caracteres.")
                 .Must(name => ServiceNameRegex.IsMatch(name))
                    .WithMessage("ServiceOfferingName contém caracteres inválidos.");
            });

            When(x => required || amountSelector(x).HasValue, () =>
            {
                RuleFor(x => amountSelector(x)!.Value)
                    .GreaterThan(0m).WithMessage("Amount deve ser maior que zero.")
                    .Must(v => decimal.Round(v, 2) == v)
                        .WithMessage("Amount deve ter no máximo 2 casas decimais.")
                    .LessThanOrEqualTo(999999999999.99m)
                        .WithMessage("Amount excede o limite permitido.");
            });

            When(x => required || !string.IsNullOrWhiteSpace(currencySelector(x)), () =>
            {
                RuleFor(x => currencySelector(x)!)
                    .Length(3).WithMessage("Currency deve ter exatamente 3 letras (ISO 4217).")
                    .Matches("^[A-Za-z]{3}$").WithMessage("Currency deve conter apenas letras (ISO 4217).");
            });

            When(x => !required && !string.IsNullOrWhiteSpace(currencySelector(x)), () =>
            {
                RuleFor(x => amountSelector(x))
                    .NotNull().WithMessage("Para trocar a moeda, informe também o Amount.");
            });

            if (positionIdsSelector is not null)
            {
                When(x => positionIdsSelector(x) is not null, () =>
                {
                    RuleFor(x => positionIdsSelector(x)!)
                        .Must(list => list!.All(id => id != Guid.Empty))
                            .WithMessage("PositionIds contém Guid vazio.")
                        .Must(list => list!.Distinct().Count() == list!.Count())
                            .WithMessage("PositionIds contém duplicatas.")
                        .Must(list => list!.Count() <= 200)
                            .WithMessage("PositionIds excede o limite de 200 itens.");
                });
            }
        }
    }
}