using EmployeeSystem.API.Controllers;
using EmployeeSystem.Data.Models;
using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeSystem.Tests.EmployeeSystem.Services.Tests.Services
{
    [TestFixture]
    public class EmployeeQueryServiceTests
    {
        private Mock<IEmployeeRepository> _repoMock = null!;
        private Mock<IEmployeeMapper> _mapperMock = null!;
        private EmployeeQueryService _service = null!;
        private Mock<ILogger<EmployeeQueryService>> _loggerMock = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IEmployeeMapper>();
            _loggerMock = new Mock<ILogger<EmployeeQueryService>>();
            _service = new EmployeeQueryService(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldMapEntitiesToDtos()
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeNumber = 1,
                    Name = "A",
                    WorkRecord = new EmployeeWorkRecord { HourlyRate = 10, HoursWorked = 5 }
                }
            };

            _repoMock.Setup(r => r.GetAllWithCountAsync(null, 1, 10))
                .ReturnsAsync((employees, 1)); 

            _mapperMock.Setup(m => m.ToDto(It.IsAny<Employee>())).Returns((Employee e) => new EmployeeResponseDto
            {
                EmployeeNumber = e.EmployeeNumber,
                Name = e.Name,
                HourlyRate = e.WorkRecord?.HourlyRate ?? 0,
                HoursWorked = e.WorkRecord?.HoursWorked ?? 0
            });

            var result = await _service.GetAllEmployeesPagedAsync(null, 1, 10);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));

            var dto = result.Data.First();
            Assert.That(dto.EmployeeNumber, Is.EqualTo(1));
            Assert.That(dto.Name, Is.EqualTo("A"));
        }


        [Test]
        public async Task GetByNumberAsync_WhenExists_ShouldReturnDto()
        {
            var entity = new Employee
            {
                EmployeeNumber = 1,
                Name = "B",
                WorkRecord = new EmployeeWorkRecord { HourlyRate = 20, HoursWorked = 8 }
            };

            var dto = new EmployeeResponseDto
            {
                EmployeeNumber = 1,
                Name = "B",
                HourlyRate = 20,
                HoursWorked = 8
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.ToDto(entity)).Returns(dto);

            var result = await _service.GetByEmployeeNumberAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("B"));
            Assert.That(result.HourlyRate, Is.EqualTo(20));
        }

        [Test]
        public async Task GetByNumberAsync_WhenNotFound_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            var result = await _service.GetByEmployeeNumberAsync(999);

            Assert.That(result, Is.Null);
        }
    }
}
