using BaitaHora.Application.Auth.Validators;
using BaitaHora.Application.Companies.Validators;
using FluentValidation;

namespace BaitaHora.Application.Auth.Commands;

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