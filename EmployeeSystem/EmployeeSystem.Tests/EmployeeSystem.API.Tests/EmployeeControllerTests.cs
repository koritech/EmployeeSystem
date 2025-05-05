using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Models;
using EmployeeSystem.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeSystem.Tests.API
{
    [TestFixture]
    public class EmployeeControllerTests
    {
        private Mock<IEmployeeService> _commandServiceMock = null!;
        private Mock<IEmployeeQueryService> _queryServiceMock = null!;
        private Mock<ILogger<EmployeesController>> _loggerMock = null!;
        private Mock<IRequestValidator> _validatorMock = null!;
        private EmployeesController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _commandServiceMock = new Mock<IEmployeeService>();
            _queryServiceMock = new Mock<IEmployeeQueryService>();
            _loggerMock = new Mock<ILogger<EmployeesController>>();
            _validatorMock = new Mock<IRequestValidator>();

            _controller = new EmployeesController(
                _commandServiceMock.Object,
                _queryServiceMock.Object,
                _loggerMock.Object,
                _validatorMock.Object);
        }

        [Test]
        public async Task GetAll_ValidInput_ReturnsOk()
        {
            _validatorMock.Setup(v => v.ValidateNumber(It.IsAny<int>(), "page")).Returns(new ValidationResult());
            _validatorMock.Setup(v => v.ValidateNumber(It.IsAny<int>(), "pageSize")).Returns(new ValidationResult());

            var employees = new List<EmployeeDto> { new() { EmployeeNumber = 1, Name = "Vivek" } };
            _queryServiceMock.Setup(s => s.GetAllEmployeesAsync(null, 1, 50)).ReturnsAsync(employees);

            var result = await _controller.GetAllEmployees(null);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var data = (result as OkObjectResult)!.Value as List<EmployeeDto>;
            Assert.That(data!.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetAll_InvalidPage_ReturnsBadRequest()
        {
            _validatorMock.Setup(v => v.ValidateNumber(0, "page"))
                .Returns(new ValidationResult { Errors = { "Invalid page number" } });
            _validatorMock.Setup(v => v.ValidateNumber(It.IsAny<int>(), "pageSize"))
                .Returns(new ValidationResult());

            var result = await _controller.GetAllEmployees(null, 0);

            var badResult = result as BadRequestObjectResult;
            Assert.That(badResult, Is.Not.Null);

            var errors = badResult!.Value!.GetType().GetProperty("Errors")!.GetValue(badResult.Value) as IEnumerable<string>;
            Assert.That(errors, Does.Contain("Invalid page number"));
        }

        [Test]
        public async Task Get_ValidId_ReturnsOk()
        {
            _validatorMock.Setup(v => v.ValidateNumber(1, "employeeNumber")).Returns(new ValidationResult());
            var dto = new EmployeeDto { EmployeeNumber = 1, Name = "John" };
            _queryServiceMock.Setup(s => s.GetByEmployeeNumberAsync(1)).ReturnsAsync(dto);

            var result = await _controller.GetEmployeeByNumber(1);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task Get_InvalidId_ReturnsBadRequest()
        {
            _validatorMock.Setup(v => v.ValidateNumber(0, "employeeNumber"))
                .Returns(new ValidationResult { Errors = { "Invalid employee number" } });

            var result = await _controller.GetEmployeeByNumber(0);

            var badResult = result as BadRequestObjectResult;
            Assert.That(badResult, Is.Not.Null);

            var errors = badResult!.Value!.GetType().GetProperty("Errors")!.GetValue(badResult.Value) as IEnumerable<string>;
            Assert.That(errors, Does.Contain("Invalid employee number"));
        }

        [Test]
        public async Task Create_ValidDto_ReturnsCreated()
        {
            var dto = new EmployeeDto { EmployeeNumber = 10, Name = "Alice" };
            _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());

            var result = await _controller.CreateEmployee(dto);

            Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
            Assert.That(((CreatedAtActionResult)result).RouteValues!["employeeNumber"], Is.EqualTo(dto.EmployeeNumber));
        }

        [Test]
        public async Task Create_InvalidDto_ReturnsBadRequest()
        {
            var dto = new EmployeeDto();
            _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult { Errors = { "Name is required" } });

            var result = await _controller.CreateEmployee(dto);

            var bad = result as BadRequestObjectResult;
            Assert.That(bad, Is.Not.Null);

            var errors = bad!.Value!.GetType().GetProperty("Errors")!.GetValue(bad.Value) as IEnumerable<string>;
            Assert.That(errors, Does.Contain("Name is required"));
        }

        [Test]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            var dto = new EmployeeDto { EmployeeNumber = 1 };

            var result = await _controller.UpdateEmployee(2, dto);

            var bad = result as BadRequestObjectResult;
            Assert.That(bad, Is.Not.Null);

            var message = bad!.Value!.GetType().GetProperty("Message")!.GetValue(bad.Value) as string;
            Assert.That(message, Is.EqualTo("Mismatched employee number."));
        }

        [Test]
        public async Task Update_InvalidDto_ReturnsBadRequest()
        {
            var dto = new EmployeeDto { EmployeeNumber = 5 };
            _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult { Errors = { "Invalid name" } });

            var result = await _controller.UpdateEmployee(5, dto);

            var bad = result as BadRequestObjectResult;
            Assert.That(bad, Is.Not.Null);

            var errors = bad!.Value!.GetType().GetProperty("Errors")!.GetValue(bad.Value) as IEnumerable<string>;
            Assert.That(errors, Does.Contain("Invalid name"));
        }

        [Test]
        public async Task Delete_InvalidId_ReturnsBadRequest()
        {
            _validatorMock.Setup(v => v.ValidateNumber(-1, "employeeNumber"))
                .Returns(new ValidationResult { Errors = { "Invalid id" } });

            var result = await _controller.DeleteEmployee(-1);
            var bad = result as BadRequestObjectResult;

            Assert.That(bad, Is.Not.Null);
            var errors = bad!.Value!.GetType().GetProperty("Errors")!.GetValue(bad.Value) as IEnumerable<string>;
            Assert.That(errors, Does.Contain("Invalid id"));
        }
    }
}
