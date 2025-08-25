namespace BaitaHora.Domain.Features.Common.Exceptions;

public sealed class SchedulingException : DomainException
{
    public SchedulingException(string message) : base(message) { }

    public SchedulingException(string message, Exception innerException)
        : base(message, innerException) { }
}