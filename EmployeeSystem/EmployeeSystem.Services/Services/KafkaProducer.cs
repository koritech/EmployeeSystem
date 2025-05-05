using Confluent.Kafka;
using Microsoft.Extensions.Logging;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IProducer<string, string> producer, ILogger<KafkaProducer> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task SendMessageAsync(string topic, string key, string value)
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = value
            });

            _logger.LogInformation($"Kafka message sent to {result.TopicPartitionOffset}");
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Kafka produce error");
            throw;
        }
    }
}
