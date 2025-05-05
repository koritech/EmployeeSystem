using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeeController> _logger;
    private readonly IRequestValidator _requestValidator;

    public EmployeeController(
        IEmployeeService employeeService,
        ILogger<EmployeeController> logger,
        IRequestValidator requestValidator)
    {
        _employeeService = employeeService;
        _logger = logger;
        _requestValidator = requestValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? name, int page = 1, int pageSize = 50)
    {
        var pageValidation = _requestValidator.ValidateNumber(page, nameof(page));
        var sizeValidation = _requestValidator.ValidateNumber(pageSize, nameof(pageSize));
        if (!pageValidation.IsValid || !sizeValidation.IsValid)
        {
            return BadRequest(new { Errors = pageValidation.Errors.Concat(sizeValidation.Errors) });
        }

        var result = await _employeeService.GetAllAsync(name, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{employeeNumber}")]
    public async Task<IActionResult> Get(int employeeNumber)
    {
        var validation = _requestValidator.ValidateNumber(employeeNumber, nameof(employeeNumber));
        if (!validation.IsValid)
        {
            return BadRequest(new { Errors = validation.Errors });
        }

        var result = await _employeeService.GetByNumberAsync(employeeNumber);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeDto dto)
    {
        var validation = _requestValidator.Validate(dto);
        if (!validation.IsValid)
        {
            return BadRequest(new { Errors = validation.Errors });
        }

        await _employeeService.AddAsync(dto);
        return CreatedAtAction(nameof(Get), new { dto.EmployeeNumber }, dto);
    }

    [HttpPut("{employeeNumber}")]
    public async Task<IActionResult> Update(int employeeNumber, [FromBody] EmployeeDto dto)
    {
        if (employeeNumber != dto.EmployeeNumber)
            return BadRequest(new { Message = "Mismatched employee number." });

        var validation = _requestValidator.Validate(dto);
        if (!validation.IsValid)
        {
            return BadRequest(new { Errors = validation.Errors });
        }

        await _employeeService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{employeeNumber}")]
    public async Task<IActionResult> Delete(int employeeNumber)
    {
        var validation = _requestValidator.ValidateNumber(employeeNumber, nameof(employeeNumber));
        if (!validation.IsValid)
        {
            return BadRequest(new { Errors = validation.Errors });
        }

        await _employeeService.DeleteAsync(employeeNumber);
        return NoContent();
    }
}
