using BaitaHora.Application.Features.Auth.Commands;
using BaitaHora.Application.Features.Companies.Validators;
using FluentValidation;

namespace BaitaHora.Application.Features.Auth.Validators;

public sealed class RegisterOwnerWithCompanyCommandValidator
    : AbstractValidator<RegisterOwnerWithCompanyCommand>
{
    public RegisterOwnerWithCompanyCommandValidator()
    {
        RuleFor(x => x.User)
            .NotNull()
            .SetValidator(new UserInputValidator());

        RuleFor(x => x.Company)
            .NotNull()
            .SetValidator(new CompanyInputValidator());
    }
}