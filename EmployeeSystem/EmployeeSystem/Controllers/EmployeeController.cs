using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

    // GET: api/employees
    [HttpGet]
    public async Task<IActionResult> GetAllEmployees([FromQuery] string? name, int page = 1, int pageSize = 50)
    {
        var pageValidation = _requestValidator.ValidateNumber(page, nameof(page));
        var sizeValidation = _requestValidator.ValidateNumber(pageSize, nameof(pageSize));

        if (!pageValidation.IsValid || !sizeValidation.IsValid)
            return BadRequest(new { Errors = pageValidation.Errors.Concat(sizeValidation.Errors) });

        var result = await _employeeQueryService.GetAllEmployeesAsync(name, page, pageSize);
        return Ok(result);
    }

    // GET: api/employees/{employeeNumber}
    [HttpGet("{employeeNumber:int}")]
    public async Task<IActionResult> GetEmployeeByNumber(int employeeNumber)
    {
        var validation = _requestValidator.ValidateNumber(employeeNumber, nameof(employeeNumber));
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors });

        var result = await _employeeQueryService.GetByEmployeeNumberAsync(employeeNumber);
        return result is null ? NotFound() : Ok(result);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto dto)
    {
        var validation = _requestValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors });

        await _employeeService.AddAsync(dto);
        return CreatedAtAction(nameof(GetEmployeeByNumber), new { employeeNumber = dto.EmployeeNumber }, dto);
    }

    // PUT: api/employees/{employeeNumber}
    [HttpPut("{employeeNumber:int}")]
    public async Task<IActionResult> UpdateEmployee(int employeeNumber, [FromBody] EmployeeDto dto)
    {
        if (employeeNumber != dto.EmployeeNumber)
            return BadRequest(new { Message = "Mismatched employee number." });

        var validation = _requestValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors });

        await _employeeService.UpdateAsync(dto);
        return NoContent();
    }

    // DELETE: api/employees/{employeeNumber}
    [HttpDelete("{employeeNumber:int}")]
    public async Task<IActionResult> DeleteEmployee(int employeeNumber)
    {
        var validation = _requestValidator.ValidateNumber(employeeNumber, nameof(employeeNumber));
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors });

        await _employeeQueryService.DeleteEmployeeAsync(employeeNumber);
        return NoContent();
    }
}
