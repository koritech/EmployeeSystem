using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using Newtonsoft.Json;

namespace EmployeeSystem.Services
{
    public class KafkaEmployeeService : IEmployeeService
    {
        private readonly IKafkaProducer _producer;
        private readonly string _topic = "employee-updates-test";

        public KafkaEmployeeService(IKafkaProducer producer)
        {
            _producer = producer;
        }

        public async Task AddAsync(EmployeeDto dto)
        {
            var message = JsonConvert.SerializeObject(dto);
            await _producer.SendMessageAsync(_topic, dto.EmployeeNumber.ToString(), message);
        }

        public async Task UpdateAsync(EmployeeDto dto)
        {
            var message = JsonConvert.SerializeObject(dto);
            await _producer.SendMessageAsync(_topic, dto.EmployeeNumber.ToString(), message);
        }
    }
}
