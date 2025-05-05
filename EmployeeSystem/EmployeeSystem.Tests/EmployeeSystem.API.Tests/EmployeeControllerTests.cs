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
        private Mock<IEmployeeService> _serviceMock = null!;
        private Mock<ILogger<EmployeeController>> _loggerMock = null!;
        private Mock<IRequestValidator> _validatorMock = null!;
        private EmployeeController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IEmployeeService>();
            _loggerMock = new Mock<ILogger<EmployeeController>>();
            _validatorMock = new Mock<IRequestValidator>();

            _controller = new EmployeeController(
                _serviceMock.Object,
                _loggerMock.Object,
                _validatorMock.Object);
        }

        [Test]
        public async Task GetAll_ValidInput_ReturnsOk()
        {
            _validatorMock.Setup(v => v.ValidateNumber(It.IsAny<int>(), "page")).Returns(new ValidationResult());
            _validatorMock.Setup(v => v.ValidateNumber(It.IsAny<int>(), "pageSize")).Returns(new ValidationResult());

            var employees = new List<EmployeeDto> { new() { EmployeeNumber = 1, Name = "Vivek" } };
            _serviceMock.Setup(s => s.GetAllAsync(null, 1, 50)).ReturnsAsync(employees);

            var result = await _controller.GetAll(null);

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

            var result = await _controller.GetAll(null, 0);

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
            _serviceMock.Setup(s => s.GetByNumberAsync(1)).ReturnsAsync(dto);

            var result = await _controller.Get(1);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task Get_InvalidId_ReturnsBadRequest()
        {
            _validatorMock.Setup(v => v.ValidateNumber(0, "employeeNumber"))
                .Returns(new ValidationResult { Errors = { "Invalid employee number" } });

            var result = await _controller.Get(0);

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

            var result = await _controller.Create(dto);

            Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
            Assert.That(((CreatedAtActionResult)result).RouteValues!["employeeNumber"], Is.EqualTo(dto.EmployeeNumber));
        }

        [Test]
        public async Task Create_InvalidDto_ReturnsBadRequest()
        {
            var dto = new EmployeeDto();
            _validatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult { Errors = { "Name is required" } });

            var result = await _controller.Create(dto);

            var bad = result as BadRequestObjectResult;
            Assert.That(bad, Is.Not.Null);

            var errors = bad!.Value!.GetType().GetProperty("Errors")!.GetValue(bad.Value) as IEnumerable<string>;
            Assert.That(errors, Does.Contain("Name is required"));
        }

        [Test]
        public async Task Update_IdMismatch_ReturnsBadRequest()
        {
            var dto = new EmployeeDto { EmployeeNumber = 1 };

            var result = await _controller.Update(2, dto);

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

            var result = await _controller.Update(5, dto);

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

            var result = await _controller.Delete(-1);
            var bad = result as BadRequestObjectResult;

            Assert.That(bad, Is.Not.Null);
            var errors = bad!.Value!.GetType().GetProperty("Errors")!.GetValue(bad.Value) as IEnumerable<string>;
            Assert.That(errors, Does.Contain("Invalid id"));
        }
    }
}
