using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services;
using Moq;
using Newtonsoft.Json;

namespace EmployeeSystem.Tests.EmployeeSystem.Services.Tests.Services
{
    [TestFixture]
    public class KafkaEmployeeServiceTests
    {
        private Mock<IKafkaProducer> _producerMock = null!;
        private KafkaEmployeeService _service = null!;
        private const string Topic = "employee-operations";

        [SetUp]
        public void SetUp()
        {
            _producerMock = new Mock<IKafkaProducer>();
            _service = new KafkaEmployeeService(_producerMock.Object);
        }

        [Test]
        public async Task AddAsync_SendsCorrectKafkaMessage()
        {
            var dto = new EmployeeDto
            {
                EmployeeNumber = 1,
                Name = "Test Employee",
                HourlyRate = 50.5m,
                HoursWorked = 40
            };

            var expectedMessage = JsonConvert.SerializeObject(dto);

            await _service.AddAsync(dto);

            _producerMock.Verify(p => p.SendMessageAsync(
                Topic,
                dto.EmployeeNumber.ToString(),
                expectedMessage), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_SendsCorrectKafkaMessage()
        {
            var dto = new EmployeeDto
            {
                EmployeeNumber = 2,
                Name = "Updated Employee",
                HourlyRate = 45.0m,
                HoursWorked = 38
            };

            var expectedMessage = JsonConvert.SerializeObject(dto);

            await _service.UpdateAsync(dto);

            _producerMock.Verify(p => p.SendMessageAsync(
                Topic,
                dto.EmployeeNumber.ToString(),
                expectedMessage), Times.Once);
        }
    }
}
