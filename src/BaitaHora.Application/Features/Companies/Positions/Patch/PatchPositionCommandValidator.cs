using BaitaHora.Application.Features.Companies.Positions.Validators;
using FluentValidation;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed class PatchPositionCommandValidator
    : PositionValidatorBase<PatchPositionCommand>
{
    public PatchPositionCommandValidator()
        : base(required: false,
               nameSelector: x => x.NewPositionName,
               levelSelector: x => x.NewAccessLevel)
    {
        RuleFor(x => x.PositionId).NotEmpty();

        RuleFor(x => x)
            .Must(x => x.NewPositionName is not null || x.NewAccessLevel.HasValue)
            .WithMessage("Informe ao menos um campo para atualização.");
    }
}