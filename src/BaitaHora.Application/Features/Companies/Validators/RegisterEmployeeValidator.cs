using BaitaHora.Application.Features.Auth.Validators;
using BaitaHora.Application.Features.Companies.Commands;
using FluentValidation;

namespace BaitaHora.Application.Features.Companies.Validators;

public sealed class RegisterEmployeeCommandValidator
    : AbstractValidator<RegisterEmployeeCommand>
{
    public RegisterEmployeeCommandValidator()
    {
        RuleFor(x => x.Employee)
            .NotNull()
            .SetValidator(new CreateUserCommandValidator());
    }
}