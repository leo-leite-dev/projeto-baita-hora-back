using System.Text.RegularExpressions;
using FluentValidation;

namespace BaitaHora.Application.Features.Auth;

public sealed class AuthenticateValidator : AbstractValidator<AuthenticateCommand>
{
    private static readonly Regex UsernameRx = new(@"^[a-zA-Z0-9._-]{3,32}$", RegexOptions.Compiled);
    private static readonly Regex EmailRx = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public AuthenticateValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .NotNull().WithMessage("Corpo da requisição é obrigatório.");

        RuleFor(x => x.Identify)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Informe e-mail ou username.")
            .MinimumLength(3).WithMessage("Identificador deve ter ao menos 3 caracteres.")
            .MaximumLength(128).WithMessage("Identificador muito longo.")
            .Must(IsEmailOrUsername).WithMessage("Informe um e-mail válido ou um username (3–32, letras/números/._-).");

        RuleFor(x => x.RawPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Informe a senha.")
            .MinimumLength(8).WithMessage("Senha deve ter pelo menos 8 caracteres.")
            .MaximumLength(128).WithMessage("Senha muito longa.")
            .Must(p => p is not null && !p.Any(char.IsWhiteSpace)).WithMessage("Senha não pode conter espaços.");
    }

    private static bool IsEmailOrUsername(string? s)
        => !string.IsNullOrWhiteSpace(s) && (EmailRx.IsMatch(s) || UsernameRx.IsMatch(s));
}