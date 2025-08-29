using FluentValidation;

namespace BaitaHora.Application.Features.Companies.Positions.Create;

public sealed class CreateCompanyPositionCommandValidator
    : CompanyPositionValidatorBase<CreateCompanyPositionCommand>
{
    public CreateCompanyPositionCommandValidator()
        : base(required: true,
               nameSelector: x => x.PositionName,
               levelSelector: x => x.AccessLevel)
    {
        RuleFor(x => x.CompanyId).NotEmpty();
    }
}