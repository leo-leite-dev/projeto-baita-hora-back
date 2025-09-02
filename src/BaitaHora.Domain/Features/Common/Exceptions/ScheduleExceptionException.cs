namespace BaitaHora.Domain.Features.Common.Exceptions;

public sealed class ScheduleException : DomainException
{
    public ScheduleException(string message) : base(message) { }

    public ScheduleException(string message, Exception innerException)
        : base(message, innerException) { }
}