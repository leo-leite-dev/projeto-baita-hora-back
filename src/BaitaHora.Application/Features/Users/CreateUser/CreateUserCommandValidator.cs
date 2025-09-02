using System.Text.RegularExpressions;
using BaitaHora.Application.Features.Users.CreateUserProfile;
using BaitaHora.Domain.Features.Common.ValueObjects;
using FluentValidation;

namespace BaitaHora.Application.Features.Users.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserEmail)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
                .WithMessage("O e-mail informado não é válido.")
            .Must(v => Email.TryParse(v, out _))
                .WithMessage("O e-mail informado não é aceito pelo domínio.")
            .MaximumLength(254);

        RuleFor(x => x.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("O username é obrigatório.")
            .MinimumLength(3).WithMessage("O username deve ter pelo menos 3 caracteres.")
            .MaximumLength(50).WithMessage("O username deve ter no máximo 50 caracteres.")
            .Must(u => Regex.IsMatch(u ?? "", "^[A-Za-z0-9._-]+$"))
                .WithMessage("O username contém caracteres inválidos. Use apenas letras, números e ._-")
            .Must(u => !Regex.IsMatch(u ?? "", @"(^[._-])|([._-]$)"))
                .WithMessage("O username não pode começar ou terminar com ., _ ou -.")
            .Must(u => !Regex.IsMatch(u ?? "", @"(\.\.|__|--)"))
                .WithMessage("O username não pode conter repetições como '..', '__' ou '--'.")
            .Must(u => Username.TryParse(u, out _))
                .WithMessage("O username informado é inválido.");

        RuleFor(x => x.RawPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.");

        RuleFor(x => x.Profile)
            .NotNull().WithMessage("O perfil do usuário é obrigatório.")
            .SetValidator(new CreateUserProfileCommandValidator());
    }
}