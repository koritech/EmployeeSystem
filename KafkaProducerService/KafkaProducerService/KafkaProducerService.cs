using Confluent.Kafka;
using Newtonsoft.Json;
using KafkaProducerService.Producer.Config;
using EmployeeService.Models; // Make sure this points to your Employee model

namespace KafkaProducerService.Producer.Services;

public class KafkaProducerService1
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducerService1(KafkaConfig config)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config.BootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        _topic = config.Topic;
    }

    public async Task ProduceAsync(Employee employee)
    {
        var key = employee.EmployeeNumber.ToString();
        var value = JsonConvert.SerializeObject(employee);

        var message = new Message<string, string> { Key = key, Value = value };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(_topic, message);
            Console.WriteLine($"Sent to Kafka: {deliveryResult.TopicPartitionOffset}");
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine($"Kafka delivery failed: {ex.Error.Reason}");
        }
    }
}
