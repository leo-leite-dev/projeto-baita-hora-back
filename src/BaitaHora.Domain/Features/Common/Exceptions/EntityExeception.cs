namespace BaitaHora.Domain.Features.Common.Exceptions;

public sealed class EntityException : DomainException
{
    public EntityException(string message) : base(message) { }

    public EntityException(string message, Exception innerException) 
        : base(message, innerException) { }
}
