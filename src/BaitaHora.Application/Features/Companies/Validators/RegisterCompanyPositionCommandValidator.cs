using System.Text.RegularExpressions;
using FluentValidation;
using BaitaHora.Application.Features.Companies.Commands;
using BaitaHora.Domain.Features.Companies.Enums;

namespace BaitaHora.Application.Features.Companies.Validators;

public sealed class RegisterCompanyPositionCommandValidator
    : AbstractValidator<RegisterCompanyPositionCommand>
{
    private static readonly Regex PositionNameRegex =
        new(@"^[\p{L}0-9 .,'\-&()/]+$", RegexOptions.Compiled);

    public RegisterCompanyPositionCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId é obrigatório.");

        RuleFor(x => x.PositionName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("PositionName é obrigatório.")
            .MinimumLength(2).WithMessage("PositionName deve ter no mínimo 2 caracteres.")
            .MaximumLength(60).WithMessage("PositionName deve ter no máximo 60 caracteres.")
            .Must(name => name is not null && PositionNameRegex.IsMatch(name))
                .WithMessage("PositionName contém caracteres inválidos.");

        RuleFor(x => x.AccessLevel)
            .Must(v => Enum.IsDefined(typeof(CompanyRole), v))
                .WithMessage("AccessLevel inválido.")
            .Must(v => v != CompanyRole.Owner && v != CompanyRole.Unknown)
                .WithMessage("Não é permitido cadastrar cargos com esses níveis de acesso.");
    }
}
