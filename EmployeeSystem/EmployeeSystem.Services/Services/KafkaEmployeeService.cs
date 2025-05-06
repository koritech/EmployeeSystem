using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace EmployeeSystem.Services
{
    public class KafkaEmployeeService : IEmployeeService
    {
        private readonly IKafkaProducer _producer;
        private readonly ILogger<KafkaEmployeeService> _logger;
        private readonly string _topic = "employee-updates-test";

        public KafkaEmployeeService(IKafkaProducer producer, ILogger<KafkaEmployeeService> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task AddAsync(EmployeeDto dto)
        {
            _logger.LogInformation("Sending new employee to Kafka");
            var message = JsonConvert.SerializeObject(dto);
            await _producer.SendMessageAsync(_topic, dto.EmployeeNumber.ToString(), message);
            _logger.LogInformation("New employee sent to Kafka");
        }

        public async Task UpdateAsync(EmployeeDto dto)
        {
            _logger.LogInformation("Sending updated employee to Kafka");
            var message = JsonConvert.SerializeObject(dto);
            await _producer.SendMessageAsync(_topic, dto.EmployeeNumber.ToString(), message);
            _logger.LogInformation("Updated employee sent to Kafka");
        }
    }
}
