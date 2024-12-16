using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class InitializationService : IHostedService
{
    private readonly ILogger<InitializationService> _logger;

    public InitializationService(ILogger<InitializationService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing the application..");
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping the application..");
        
        return Task.CompletedTask;
    }
}