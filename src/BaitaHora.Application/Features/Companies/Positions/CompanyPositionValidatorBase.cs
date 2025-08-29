using FluentValidation;
using System.Text.RegularExpressions;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Features.Companies.Positions;

public abstract class CompanyPositionValidatorBase<T> : AbstractValidator<T>
{
    private static readonly Regex PositionNameRegex =
        new(@"^[\p{L}0-9 .,'\-&()/]+$", RegexOptions.Compiled);

    protected CompanyPositionValidatorBase(
        bool required,
        Func<T, string?> nameSelector,
        Func<T, CompanyRole?> levelSelector)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        // PositionName
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

        // AccessLevel
        When(x => required || levelSelector(x).HasValue, () =>
        {
            RuleFor(x => levelSelector(x)!.Value)
                .Must(v => Enum.IsDefined(typeof(CompanyRole), v))
                    .WithMessage("AccessLevel inválido.")
                .Must(v => v != CompanyRole.Owner && v != CompanyRole.Unknown)
                    .WithMessage("Não é permitido usar esse nível de acesso.");
        });
    }
}