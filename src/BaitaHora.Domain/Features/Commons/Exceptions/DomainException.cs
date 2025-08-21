namespace BaitaHora.Domain.Features.Commons.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}