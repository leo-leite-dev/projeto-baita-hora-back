using Microsoft.EntityFrameworkCore.Diagnostics;

public sealed class TestSaveChangesInterceptor : SaveChangesInterceptor
{
    public int Calls { get; private set; }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Calls++;
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
