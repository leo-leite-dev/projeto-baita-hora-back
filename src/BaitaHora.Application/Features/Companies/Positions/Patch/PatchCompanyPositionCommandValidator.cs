using FluentValidation;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed class PatchCompanyPositionCommandValidator
    : CompanyPositionValidatorBase<PatchCompanyPositionCommand>
{
    public PatchCompanyPositionCommandValidator()
        : base(required: false,
               nameSelector: x => x.NewPositionName,
               levelSelector: x => x.NewAccessLevel)
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.PositionId).NotEmpty();

        RuleFor(x => x)
            .Must(x => x.NewPositionName is not null || x.NewAccessLevel.HasValue)
            .WithMessage("Informe ao menos um campo para atualização.");
    }
}