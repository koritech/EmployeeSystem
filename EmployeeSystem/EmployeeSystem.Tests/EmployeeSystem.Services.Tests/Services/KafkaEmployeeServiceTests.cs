﻿using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Services;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

namespace EmployeeSystem.Tests.Services
{
    [TestFixture]
    public class KafkaEmployeeServiceTests
    {
        private const string Topic = "employee-updates-test";

        private Mock<IKafkaProducer> _producerMock = null!;
        private Mock<Microsoft.Extensions.Logging.ILogger<KafkaEmployeeService>> _loggerMock = null!;
        private Mock<IOptions<KafkaSettings>> _optionsMock = null!;
        private KafkaEmployeeService _service = null!;

        [SetUp]
        public void Setup()
        {
            var kafkaSettings = new KafkaSettings
            {
                EmployeeTopic = "test-topic"
            };
            _producerMock = new Mock<IKafkaProducer>();
            _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<KafkaEmployeeService>>();
            _optionsMock.Setup(x => x.Value).Returns(kafkaSettings);
            _service = new KafkaEmployeeService(_producerMock.Object, _loggerMock.Object, _optionsMock.Object);
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

    internal interface ILogger<T>
    {
    }
}
