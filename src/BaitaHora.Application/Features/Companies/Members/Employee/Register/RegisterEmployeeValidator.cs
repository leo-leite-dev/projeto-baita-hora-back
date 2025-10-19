using BaitaHora.Application.Features.Users.CreateUser;
using FluentValidation;

namespace BaitaHora.Application.Features.Companies.Members.Employee.Register;

public sealed class RegisterEmployeeCommandValidator
    : AbstractValidator<RegisterMemberCommand>
{
    public RegisterEmployeeCommandValidator()
    {
        RuleFor(x => x.Employee)
            .NotNull()
            .SetValidator(new CreateUserCommandValidator());
    }
}