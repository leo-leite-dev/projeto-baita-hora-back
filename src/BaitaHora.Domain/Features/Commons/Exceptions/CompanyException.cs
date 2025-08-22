namespace BaitaHora.Domain.Features.Commons.Exceptions;

public sealed class CompanyException : DomainException
{
    public CompanyException(string message) : base(message) { }

    public CompanyException(string message, Exception innerException) 
        : base(message, innerException) { }
}
