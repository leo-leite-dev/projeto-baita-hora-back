using BaitaHora.Application.Features.Companies.Positions.Validators;
using FluentValidation;

namespace BaitaHora.Application.Features.Companies.Positions.Patch;

public sealed class PatchPositionCommandValidator
    : PositionValidatorBase<PatchPositionCommand>
{
    public PatchPositionCommandValidator()
        : base(required: false,
               nameSelector: x => x.PositionName,
               levelSelector: x => x.AccessLevel)
    {
        RuleFor(x => x.PositionId).NotEmpty();

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.PositionName)
                || x.AccessLevel.HasValue
                || x.SetServiceOfferingIds is not null)
            .WithMessage("Informe ao menos um campo para atualização.");
    }
}