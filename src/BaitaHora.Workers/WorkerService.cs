public sealed class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly IConfiguration _cfg;

    public WorkerService(ILogger<WorkerService> logger, IConfiguration cfg)
    {
        _logger = logger;
        _cfg = cfg;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Worker iniciado.");

        if (_cfg.GetValue("Worker:SeedOwner", false))
        {
            // chamar seu método de seed aqui (o que monta RegisterOwnerWithCompanyRequest)
            // await SeedOwnerAsync(ct);
        }

        while (!ct.IsCancellationRequested)
        {
            _logger.LogInformation("Worker heartbeat...");
            await Task.Delay(TimeSpan.FromSeconds(10), ct);
        }
    }
}
