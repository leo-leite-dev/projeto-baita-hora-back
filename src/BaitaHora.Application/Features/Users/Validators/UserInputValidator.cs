using System.Text.RegularExpressions;
using BaitaHora.Application.Features.Commons.Validators;
using BaitaHora.Application.Features.Users.DTOs;
using BaitaHora.Domain.Features.Users.ValueObjects;
using FluentValidation;

namespace BaitaHora.Application.Features.Auth.Validators;

public sealed class UserInputValidator : AbstractValidator<UserInput>
{
    public UserInputValidator()
    {
        RuleFor(x => x.Email).EmailVo();

        RuleFor(x => x.Username)
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
                .WithMessage("Username inválido.");

        RuleFor(x => x.RawPassword)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.");

        RuleFor(x => x.Profile)
            .NotNull().WithMessage("O perfil do usuário é obrigatório.")
            .SetValidator(new UserProfileInputValidator());
    }
}