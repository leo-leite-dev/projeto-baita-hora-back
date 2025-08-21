using BaitaHora.Application.Auth.Commands;
using FluentValidation;

namespace BaitaHora.Application.Auth.Validators;

public sealed class AuthenticateValidator : AbstractValidator<AuthenticateCommand>
{
    public AuthenticateValidator()
    {
        RuleFor(x => x.Input.Identify).NotEmpty();
        RuleFor(x => x.Input.Password).NotEmpty();
    }
}
