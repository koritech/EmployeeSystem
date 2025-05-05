using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Services;
using Moq;

namespace EmployeeSystem.Tests.EmployeeSystem.Services.Tests.Services
{
    [TestFixture]
    public class EmployeeQueryServiceTests
    {
        private Mock<IEmployeeRepository> _repoMock = null!;
        private Mock<IEmployeeMapper> _mapperMock = null!;
        private EmployeeQueryService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IEmployeeMapper>();
            _service = new EmployeeQueryService(_repoMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldMapEntitiesToDtos()
        {
            var employees = new List<Employee>
            {
                new Employee { EmployeeNumber = 1, Name = "A", HourlyRate = 10, HoursWorked = 5 }
            };

            _repoMock.Setup(r => r.GetAllAsync(null, 1, 10)).ReturnsAsync(employees);

            _mapperMock.Setup(m => m.ToDto(It.IsAny<Employee>())).Returns((Employee e) => new EmployeeDto
            {
                EmployeeNumber = e.EmployeeNumber,
                Name = e.Name,
                HourlyRate = e.HourlyRate,
                HoursWorked = e.HoursWorked
            });

            var result = await _service.GetAllAsync(null, 1, 10);

            Assert.That(result.Count(), Is.EqualTo(1));
            var dto = result.First();
            Assert.That(dto.Name, Is.EqualTo("A"));
        }

        [Test]
        public async Task GetByNumberAsync_WhenExists_ShouldReturnDto()
        {
            var entity = new Employee { EmployeeNumber = 1, Name = "B", HourlyRate = 20, HoursWorked = 8 };
            var dto = new EmployeeDto { EmployeeNumber = 1, Name = "B", HourlyRate = 20, HoursWorked = 8 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.ToDto(entity)).Returns(dto);

            var result = await _service.GetByNumberAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("B"));
        }

        [Test]
        public async Task GetByNumberAsync_WhenNotFound_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            var result = await _service.GetByNumberAsync(999);

            Assert.That(result, Is.Null);
        }
    }
}
