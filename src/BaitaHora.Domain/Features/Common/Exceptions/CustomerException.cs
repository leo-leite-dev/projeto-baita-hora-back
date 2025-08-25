namespace BaitaHora.Domain.Features.Common.Exceptions;

public sealed class CustomerException : DomainException
{
    public CustomerException(string message) : base(message) { }

    public CustomerException(string message, Exception innerException)
        : base(message, innerException) { }
}