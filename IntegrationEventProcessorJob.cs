

internal class IntegrationEventProcessorJob : BackgroundService
{
    private readonly IConsumer<UserCreateOrderIntegrationEvent> _messageQueue;

    readonly ILogger<IntegrationEventProcessorJob> _logger;
  

    public IntegrationEventProcessorJob(IConsumer<UserCreateOrderIntegrationEvent> messageQueue, ILogger<IntegrationEventProcessorJob> logger)
    {
        _messageQueue = messageQueue;

        _logger = logger;
          
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
         await foreach (var @event in this._messageQueue.ReadMessageStreamAsync(stoppingToken))
        {
            _logger.LogInformation($"One System Out Processing event: {@event}---{this.GetHashCode()}");
        }
    }
}





