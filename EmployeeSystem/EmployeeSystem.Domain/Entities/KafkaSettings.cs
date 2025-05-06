namespace EmployeeSystem.Domain.Entities
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;

        public string EmployeeTopic { get; set; }
    }

}
