namespace BaitaHora.Infrastructure.Data.Outbox;
public enum OutboxStatus : short
{
    Pending   = 0,
    InFlight  = 1,
    Published = 2,
    Failed    = 3
}
