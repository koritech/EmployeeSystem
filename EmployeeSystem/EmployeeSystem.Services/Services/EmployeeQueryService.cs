using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Models;
using Microsoft.Extensions.Logging;

namespace EmployeeSystem.Services.Services
{
    public class EmployeeQueryService : IEmployeeQueryService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IEmployeeMapper _mapper;
        private readonly ILogger<EmployeeQueryService> _logger;

        public EmployeeQueryService(IEmployeeRepository repo, IEmployeeMapper mapper, ILogger<EmployeeQueryService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<EmployeePagedResult> GetAllEmployeesPagedAsync(string? nameFilter, int page, int pageSize)
        {
            _logger.LogInformation("Getting paged employees");
            var (data, totalCount) = await _repo.GetAllWithCountAsync(nameFilter, page, pageSize);

            _logger.LogInformation("Paged employees fetched");
            return new EmployeePagedResult
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data.Select(_mapper.ToDto)
            };
        }

        public async Task<EmployeeResponseDto?> GetByEmployeeNumberAsync(int employeeNumber)
        {
            _logger.LogInformation("Getting employee by number");

            var entity = await _repo.GetByIdAsync(employeeNumber);
            if (entity is null)
            {
                _logger.LogInformation("Employee not found");
                return null;
            }

            _logger.LogInformation("Employee found");
            return _mapper.ToDto(entity);
        }

        public async Task DeleteEmployeeAsync(int employeeNumber)
        {
            _logger.LogInformation("Deleting employee");

            var employee = await _repo.GetByIdAsync(employeeNumber);
            if (employee is null)
            {
                _logger.LogInformation("Employee not found");
                return;
            }

            await _repo.DeleteAsync(employee);
            await _repo.SaveChangesAsync();

            _logger.LogInformation("Employee deleted");
        }
    }
}
