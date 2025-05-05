namespace KafkaProducerService.Producer.Config;

public class KafkaConfig
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "employee-updates";
}
