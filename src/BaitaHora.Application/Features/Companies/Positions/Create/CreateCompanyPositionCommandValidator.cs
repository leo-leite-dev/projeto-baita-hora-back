using BaitaHora.Application.Features.Companies.Positions.Validators;
using FluentValidation;

namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed class CreatePositionCommandValidator
    : PositionValidatorBase<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
        : base(required: true,
               nameSelector: x => x.PositionName,
               levelSelector: x => x.AccessLevel)
    {
        RuleFor(x => x.CompanyId).NotEmpty();
    }
}