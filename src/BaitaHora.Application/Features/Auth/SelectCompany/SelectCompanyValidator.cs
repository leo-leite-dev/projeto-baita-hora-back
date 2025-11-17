using FluentValidation;

namespace BaitaHora.Application.Features.Auth.SelectCompany;

public sealed class SelectCompanyValidator : AbstractValidator<SelectCompanyCommand>
{
    public SelectCompanyValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Informe a empresa a selecionar.");
    }
}