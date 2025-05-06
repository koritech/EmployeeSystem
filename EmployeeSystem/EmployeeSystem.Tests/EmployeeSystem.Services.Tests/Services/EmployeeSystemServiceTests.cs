using EmployeeSystem.Data.Models;
using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Services;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeSystem.Tests.Services
{
    [TestFixture]
    public class EmployeeSystemServiceTests
    {
        private Mock<IEmployeeRepository> _repoMock = null!;
        private Mock<IEmployeeMapper> _mapperMock = null!;
        private Mock<Microsoft.Extensions.Logging.ILogger<DbEmployeeService>> _loggerMock = null!;
        private DbEmployeeService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IEmployeeMapper>();
            _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<DbEmployeeService>>();
            _service = new DbEmployeeService(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
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
                WorkRecord = new EmployeeWorkRecord
                {
                    EmployeeNumber = 1,
                    HourlyRate = 50,
                    HoursWorked = 40
                }
            };

            _mapperMock.Setup(m => m.ToEntity(dto)).Returns(entity);

            await _service.AddAsync(dto);

            _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_WhenExists_ShouldUpdateAndSave()
        {
            var emp = new Employee
            {
                EmployeeNumber = 1,
                Name = "Old",
                WorkRecord = new EmployeeWorkRecord
                {
                    EmployeeNumber = 1,
                    HourlyRate = 10,
                    HoursWorked = 5
                }
            };

            var dto = new EmployeeDto
            {
                EmployeeNumber = 1,
                Name = "New",
                HourlyRate = 20,
                HoursWorked = 10
            };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emp);

            _mapperMock.Setup(m => m.UpdateEntity(emp, dto)).Callback(() =>
            {
                emp.Name = dto.Name;
                if (emp.WorkRecord != null)
                {
                    emp.WorkRecord.HourlyRate = dto.HourlyRate;
                    emp.WorkRecord.HoursWorked = dto.HoursWorked;
                }
            });

            await _service.UpdateAsync(dto);

            Assert.That(emp.Name, Is.EqualTo("New"));
            Assert.That(emp.WorkRecord!.HourlyRate, Is.EqualTo(20));
            Assert.That(emp.WorkRecord.HoursWorked, Is.EqualTo(10));

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
