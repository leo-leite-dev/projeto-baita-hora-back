using BaitaHora.Application.Features.Companies.Common.Create;
using BaitaHora.Application.Features.Users.CreateUser;
using FluentValidation;

namespace BaitaHora.Application.Features.Onboarding;

public sealed class RegisterOwnerWithCompanyCommandValidator
    : AbstractValidator<RegisterOwnerWithCompanyCommand>
{
    public RegisterOwnerWithCompanyCommandValidator()
    {
        RuleFor(x => x.Owner)
            .NotNull()
            .SetValidator(new CreateUserCommandValidator());

        RuleFor(x => x.Company)
            .NotNull()
            .SetValidator(new CreateCompanyBaseCommandValidator());
    }
}