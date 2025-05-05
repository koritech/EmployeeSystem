using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using Moq;

namespace EmployeeSystem.Tests.Services
{
    [TestFixture]
    public class EmployeeSystemServiceTests
    {
        private Mock<IEmployeeRepository> _repoMock = null!;
        private Mock<IEmployeeMapper> _mapperMock = null!;
        private DbEmployeeService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IEmployeeMapper>();
            _service = new DbEmployeeService(_repoMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task AddAsync_ShouldCallRepoAddAndSave()
        {
            var dto = new EmployeeDto
            {
                EmployeeNumber = 1,
                Name = "Vivek",
                HourlyRate = 50,
                HoursWorked = 40
            };

            var entity = new Employee
            {
                EmployeeNumber = 1,
                Name = "Vivek",
                HourlyRate = 50,
                HoursWorked = 40
            };

            _mapperMock.Setup(m => m.ToEntity(dto)).Returns(entity);

            await _service.AddAsync(dto);

            _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_WhenExists_ShouldCallRepoDeleteAndSave()
        {
            var emp = new Employee { EmployeeNumber = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emp);

            await _service.DeleteAsync(1);

            _repoMock.Verify(r => r.DeleteAsync(emp), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_WhenNotFound_ShouldDoNothing()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            await _service.DeleteAsync(999);

            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Employee>()), Times.Never);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task GetAllAsync_ShouldMapEntitiesToDtos()
        {
            var employees = new List<Employee>
            {
                new Employee { EmployeeNumber = 1, Name = "A", HourlyRate = 10, HoursWorked = 5 }
            };

            var dtos = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeNumber = 1, Name = "A", HourlyRate = 10, HoursWorked = 5 }
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
            Assert.That(dto.EmployeeNumber, Is.EqualTo(1));
            Assert.That(dto.Name, Is.EqualTo("A"));
        }

        [Test]
        public async Task GetByNumberAsync_WhenExists_ShouldReturnDto()
        {
            var employee = new Employee
            {
                EmployeeNumber = 1,
                Name = "Test",
                HourlyRate = 50,
                HoursWorked = 40
            };

            var expectedDto = new EmployeeDto
            {
                EmployeeNumber = 1,
                Name = "Test",
                HourlyRate = 50,
                HoursWorked = 40
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _mapperMock.Setup(m => m.ToDto(employee)).Returns(expectedDto);

            var dto = await _service.GetByNumberAsync(1);

            Assert.IsNotNull(dto);
            Assert.That(dto!.Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task GetByNumberAsync_WhenNotFound_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((Employee?)null);

            var result = await _service.GetByNumberAsync(123);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateAsync_WhenExists_ShouldUpdateAndSave()
        {
            var emp = new Employee { EmployeeNumber = 1, Name = "Old", HourlyRate = 10, HoursWorked = 5 };
            var dto = new EmployeeDto { EmployeeNumber = 1, Name = "New", HourlyRate = 20, HoursWorked = 10 };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emp);
            _mapperMock.Setup(m => m.UpdateEntity(emp, dto)).Callback(() =>
            {
                emp.Name = dto.Name;
                emp.HourlyRate = dto.HourlyRate;
                emp.HoursWorked = dto.HoursWorked;
            });

            await _service.UpdateAsync(dto);

            Assert.That(emp.Name, Is.EqualTo("New"));
            Assert.That(emp.HourlyRate, Is.EqualTo(20));
            Assert.That(emp.HoursWorked, Is.EqualTo(10));

            _repoMock.Verify(r => r.UpdateAsync(emp), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_WhenNotFound_ShouldDoNothing()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            var dto = new EmployeeDto { EmployeeNumber = 999, Name = "X", HourlyRate = 0, HoursWorked = 0 };

            await _service.UpdateAsync(dto);

            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Never);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
