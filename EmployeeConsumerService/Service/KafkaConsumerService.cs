using Confluent.Kafka;
using EmployeeConsumerService.Domain.Interfaces;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using Newtonsoft.Json;

namespace Application.Services;

public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly IEmployeeService _employeeService;
    private readonly string _bootstrapServers = "localhost:9092";
    private readonly string _topic = "employee-updatez";

    public KafkaConsumerService(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "employee-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);

                    Console.WriteLine($"[Kafka] Received: {result.Message.Value}");

                    var dto = JsonConvert.DeserializeObject<EmployeeDto>(result.Message.Value);

                    if (dto != null)
                    {
                        await _employeeService.AddAsync(dto);
                        Console.WriteLine($"[DB] Saved employee #{dto.EmployeeNumber}");
                        consumer.Commit(result);
                    }
                    else
                    {
                        Console.WriteLine("[Error] Failed to deserialize message.");
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"[Kafka Error] {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Processing Error] {ex.Message}");
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}
