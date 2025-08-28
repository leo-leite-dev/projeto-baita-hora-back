using System.Text.RegularExpressions;
using FluentValidation;
using BaitaHora.Application.Features.Commons.Validators;
using BaitaHora.Application.Features.Users.PatchUserProfile;

namespace BaitaHora.Application.Features.Users.PatchUser;

public sealed class PatchUserCommandValidator : AbstractValidator<PatchUserCommand>
{
    public PatchUserCommandValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.UserEmail), () =>
        {
            RuleFor(x => x.UserEmail!).EmailVo();
        });

        When(x => !string.IsNullOrWhiteSpace(x.Username), () =>
        {
            RuleFor(x => x.Username!)
                .MinimumLength(3).WithMessage("O username deve ter pelo menos 3 caracteres.")
                .MaximumLength(50).WithMessage("O username deve ter no máximo 50 caracteres.")
                .Must(u => Regex.IsMatch(u, "^[A-Za-z0-9._-]+$"))
                    .WithMessage("Use apenas letras, números e os símbolos . _ -")
                .Must(u => !Regex.IsMatch(u, @"(^[._-])|([._-]$)"))
                    .WithMessage("O username não pode começar ou terminar com ., _ ou -.")
                .Must(u => !Regex.IsMatch(u, @"(\.\.|__|--)"))
                    .WithMessage("O username não pode conter '..', '__' ou '--'.")
                .Must(u => Username.TryParse(u, out _))
                    .WithMessage("Username inválido.");
        });

        When(x => x.Profile is not null, () =>
        {
            RuleFor(x => x.Profile!).SetValidator(new PatchUserProfileCommandValidator());
        });
    }
}