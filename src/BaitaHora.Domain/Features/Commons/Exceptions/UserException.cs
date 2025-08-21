namespace BaitaHora.Domain.Features.Commons.Exceptions;

public class UserException : DomainException
{
    public UserException(string message) : base(message) { }
}
