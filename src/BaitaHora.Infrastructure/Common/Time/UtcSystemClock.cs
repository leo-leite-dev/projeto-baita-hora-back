using BaitaHora.Application.Common.Time;

namespace BaitaHora.Infrastructure.Common.Time;

public sealed class UtcSystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}