public interface IKafkaProducer
{
    Task SendMessageAsync(string topic, string key, string value);
}