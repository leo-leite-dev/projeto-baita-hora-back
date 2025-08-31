using FluentValidation;
using System.Text.RegularExpressions;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Features.Companies.Positions.Validators;

public abstract class PositionValidatorBase<T> : AbstractValidator<T>
{
    private static readonly Regex PositionNameRegex =
        new(@"^[\p{L}0-9 .,'\-&()/]+$", RegexOptions.Compiled);

    protected PositionValidatorBase(
        bool required,
        Func<T, string?> nameSelector,
        Func<T, CompanyRole?> levelSelector)
        : this(required, nameSelector, levelSelector, servicesSelector: null, servicesRequiredPredicate: null)
    { }

    protected PositionValidatorBase(
        bool required,
        Func<T, string?> nameSelector,
        Func<T, CompanyRole?> levelSelector,
        Func<T, IEnumerable<Guid>?>? servicesSelector,
        Func<T, bool>? servicesRequiredPredicate)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        When(x => required || nameSelector(x) is not null, () =>
        {
            var r = RuleFor(x => nameSelector(x)!);
            if (required)
                r.NotEmpty().WithMessage("PositionName é obrigatório.");
            else
                r.NotEmpty().WithMessage("PositionName não pode ser vazio.");

            r.MinimumLength(2).WithMessage("PositionName deve ter no mínimo 2 caracteres.")
             .MaximumLength(60).WithMessage("PositionName deve ter no máximo 60 caracteres.")
             .Must(name => PositionNameRegex.IsMatch(name))
                .WithMessage("PositionName contém caracteres inválidos.");
        });

        When(x => required || levelSelector(x).HasValue, () =>
        {
            RuleFor(x => levelSelector(x)!.Value)
                .Must(v => Enum.IsDefined(typeof(CompanyRole), v))
                    .WithMessage("AccessLevel inválido.")
                .Must(v => v != CompanyRole.Owner && v != CompanyRole.Unknown)
                    .WithMessage("Não é permitido usar esse nível de acesso.");
        });

        if (servicesSelector is not null)
        {
            When(x => servicesSelector(x) is not null, () =>
            {
                RuleForEach(x => servicesSelector(x)!)
                    .Must(id => id != Guid.Empty)
                    .WithMessage("ServiceOfferingId inválido.");
            });

            if (servicesRequiredPredicate is not null)
            {
                When(x => servicesRequiredPredicate(x) && servicesSelector(x) is not null, () =>
                {
                    RuleFor(x => servicesSelector(x)!)
                        .Must(ids => ids.Any(id => id != Guid.Empty))
                        .WithMessage("Informe ao menos um serviço válido para este cargo.");
                });
            }
        }
    }
}