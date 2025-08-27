namespace BaitaHora.Application.Common.Errors;

public sealed class UniqueConstraintViolationException : Exception
{
    public string? ConstraintName { get; }
    public UniqueConstraintViolationException(string? message = null, string? constraintName = null, Exception? inner = null)
        : base(message ?? "Violação de unicidade.", inner)
        => ConstraintName = constraintName;
}

public sealed class ForeignKeyConstraintViolationException : Exception
{
    public string? ConstraintName { get; }
    public ForeignKeyConstraintViolationException(string? message = null, string? constraintName = null, Exception? inner = null)
        : base(message ?? "Violação de integridade referencial.", inner)
        => ConstraintName = constraintName;
}