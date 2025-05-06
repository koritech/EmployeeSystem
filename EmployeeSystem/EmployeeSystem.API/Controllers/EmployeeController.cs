using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Validation;
using Microsoft.AspNetCore.Mvc;


namespace EmployeeSystem.API.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeQueryService _employeeQueryService;
        private readonly ILogger<EmployeesController> _logger;
        private readonly IRequestValidator _requestValidator;

        public EmployeesController(
            IEmployeeService employeeService,
            IEmployeeQueryService employeeQueryService,
            ILogger<EmployeesController> logger,
            IRequestValidator requestValidator)
        {
            _employeeService = employeeService;
            _employeeQueryService = employeeQueryService;
            _logger = logger;
            _requestValidator = requestValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] string? name, int page = 1, int pageSize = 50)
        {
            _logger.LogInformation("GetAllEmployees called");

            var pageValidation = _requestValidator.ValidateNumber(page, nameof(page));
            var sizeValidation = _requestValidator.ValidatePageSize(pageSize, nameof(pageSize));

            if (!pageValidation.IsValid || !sizeValidation.IsValid)
            {
                _logger.LogWarning("Invalid pagination parameters");
                return BadRequest(new { Errors = pageValidation.Errors.Concat(sizeValidation.Errors) });
            }

            var result = await _employeeQueryService.GetAllEmployeesPagedAsync(name, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{employeeNumber:int}")]
        public async Task<IActionResult> GetEmployeeByNumber(int employeeNumber)
        {
            _logger.LogInformation($"GetEmployeeByNumber called with ID: {employeeNumber}");

            var validation = _requestValidator.ValidateNumber(employeeNumber, nameof(employeeNumber));
            if (!validation.IsValid)
            {
                _logger.LogWarning("Invalid employee number");
                return BadRequest(new { Errors = validation.Errors });
            }

            var result = await _employeeQueryService.GetByEmployeeNumberAsync(employeeNumber);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto dto)
        {
            _logger.LogInformation("CreateEmployee called");

            var validation = _requestValidator.Validate(dto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for employee creation");
                return BadRequest(new { Errors = validation.Errors });
            }

            await _employeeService.AddAsync(dto);
            _logger.LogInformation("Employee created successfully");

            return CreatedAtAction(nameof(GetEmployeeByNumber), new { employeeNumber = dto.EmployeeNumber }, dto);
        }

        [HttpPut("{employeeNumber:int}")]
        public async Task<IActionResult> UpdateEmployee(int employeeNumber, [FromBody] EmployeeDto dto)
        {
            _logger.LogInformation("UpdateEmployee called");

            if (employeeNumber != dto.EmployeeNumber)
            {
                _logger.LogWarning("Mismatched employee number");
                return BadRequest(new { Message = "Mismatched employee number." });
            }

            var validation = _requestValidator.Validate(dto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for employee update");
                return BadRequest(new { Errors = validation.Errors });
            }

            await _employeeService.UpdateAsync(dto);
            _logger.LogInformation("Employee updated successfully");

            return NoContent();
        }

        [HttpDelete("{employeeNumber:int}")]
        public async Task<IActionResult> DeleteEmployee(int employeeNumber)
        {
            _logger.LogInformation($"DeleteEmployee called for ID: {employeeNumber}");

            var validation = _requestValidator.ValidateNumber(employeeNumber, nameof(employeeNumber));
            if (!validation.IsValid)
            {
                _logger.LogWarning("Invalid employee number for delete");
                return BadRequest(new { Errors = validation.Errors });
            }

            await _employeeQueryService.DeleteEmployeeAsync(employeeNumber);
            _logger.LogInformation("Employee deleted successfully");

            return NoContent();
        }
    }
}
