namespace BaitaHora.Application.Common.Errors;

public interface IDbErrorTranslator
{
    string? TryTranslateUniqueViolation(string? constraintName, string? detail = null);
}