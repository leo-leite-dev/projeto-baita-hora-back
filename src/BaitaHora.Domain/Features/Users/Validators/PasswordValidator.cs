using System.Text.RegularExpressions;
using BaitaHora.Domain.Features.Commons.Exceptions;

namespace BaitaHora.Domain.Features.Users.Validators;

public static class PasswordValidator
{
    public static void EnsureStrength(string? password)
    {
        if (password is null) throw new UserException("Senha inválida."); 
        if (password.Length < 8)
            throw new UserException("A senha deve conter no mínimo 8 caracteres.");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            throw new UserException("A senha deve conter pelo menos uma letra maiúscula.");

        if (!Regex.IsMatch(password, @"[a-z]"))
            throw new UserException("A senha deve conter pelo menos uma letra minúscula.");

        if (!Regex.IsMatch(password, @"\d"))
            throw new UserException("A senha deve conter pelo menos um número.");

        if (!Regex.IsMatch(password, @"[\W_]"))
            throw new UserException("A senha deve conter pelo menos um caractere especial (ex: ! @ # $ %).");

        if (password.Contains(" "))
            throw new UserException("A senha não pode conter espaços.");
    }
}

//Senha: não como VO de texto; continue validando via política e persista apenas o hash (pode ser um VO PasswordHash).