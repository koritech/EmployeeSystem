using Confluent.Kafka;
using EmployeeConsumerService.Domain.Interfaces;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using Newtonsoft.Json;

namespace Application.Services;

public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly IEmployeeService _employeeService;
    private readonly IEmployeeQueryService _employeeQueryService;
    private readonly string _bootstrapServers = "localhost:9092";
    private readonly string _topic = "employee-updates-test";

    public KafkaConsumerService(IEmployeeService employeeService, IEmployeeQueryService employeeQueryService)
    {
        _employeeService = employeeService;
        _employeeQueryService = employeeQueryService;
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "employee-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 6000
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result = null;

                try
                {
                    result = consumer.Consume(cancellationToken);
                    if (result?.Message?.Value is null)
                    {
                        Console.WriteLine("[Kafka] Null or empty message received.");
                        continue;
                    }

                    Console.WriteLine($"[Kafka] Received: {result.Message.Value}");

                    var dto = JsonConvert.DeserializeObject<EmployeeDto>(result.Message.Value);

                    if (dto is null)
                    {
                        Console.WriteLine("[Error] Failed to deserialize Kafka message.");
                        continue;
                    }

                    var existing = await _employeeQueryService.GetByNumberAsync(dto.EmployeeNumber);
                    if (existing != null)
                    {
                        await _employeeService.UpdateAsync(dto);
                    }
                    else
                    {
                        await _employeeService.AddAsync(dto);
                    }

                    Console.WriteLine($"[DB] Processed employee #{dto.EmployeeNumber}");

                    try
                    {
                        consumer.Commit(result);
                    }
                    catch (KafkaException kex)
                    {
                        Console.WriteLine($"[Kafka Commit Error] {kex.Message}");
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"[Kafka Error] {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Processing Error] {ex}");
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}
