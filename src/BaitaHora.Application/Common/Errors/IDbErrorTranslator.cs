using Npgsql;

namespace BaitaHora.Application.Common.Errors;

public interface IDbErrorTranslator
{
    string? TryTranslateUniqueViolation(PostgresException ex);
}
