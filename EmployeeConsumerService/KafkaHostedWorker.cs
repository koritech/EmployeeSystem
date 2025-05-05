using EmployeeConsumerService.Domain.Interfaces;

public class KafkaHostedWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public KafkaHostedWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var consumerService = scope.ServiceProvider.GetRequiredService<IKafkaConsumerService>();
        await consumerService.StartConsumingAsync(stoppingToken);
    }
}