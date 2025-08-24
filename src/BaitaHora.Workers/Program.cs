
var builder = Host.CreateDefaultBuilder(args);

if (OperatingSystem.IsWindows())
    builder = builder.UseWindowsService();
else if (OperatingSystem.IsLinux())
    builder = builder.UseSystemd();

using var host = builder
    .ConfigureServices(s => s.AddHostedService<WorkerService>())
    .Build();

await host.RunAsync();
