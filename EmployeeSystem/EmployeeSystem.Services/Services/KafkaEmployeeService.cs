using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EmployeeSystem.Services
{
    public class KafkaEmployeeService : IEmployeeService
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<KafkaEmployeeService> _logger;
        private readonly KafkaSettings _kafkaSettings ;

        public KafkaEmployeeService(IKafkaProducer producer, ILogger<KafkaEmployeeService> logger, IOptions<KafkaSettings> kafkaSettings)
        {
            _producer = producer;
            _logger = logger;
            _kafkaSettings = kafkaSettings.Value;
        }

        public async Task AddAsync(EmployeeDto dto)
        {
            _logger.LogInformation("Sending new employee to Kafka");
            var message = JsonConvert.SerializeObject(dto);
            await _producer.SendMessageAsync(_kafkaSettings.EmployeeTopic, dto.EmployeeNumber.ToString(), message);
            _logger.LogInformation("New employee sent to Kafka");
        }

        public async Task UpdateAsync(EmployeeDto dto)
        {
            _logger.LogInformation("Sending updated employee to Kafka");
            var message = JsonConvert.SerializeObject(dto);
            await _producer.SendMessageAsync(_kafkaSettings.EmployeeTopic, dto.EmployeeNumber.ToString(), message);
            _logger.LogInformation("Updated employee sent to Kafka");
        }
    }
}
