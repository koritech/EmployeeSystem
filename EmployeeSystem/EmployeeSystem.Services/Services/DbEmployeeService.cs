using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services.Mappers;
using Microsoft.Extensions.Logging;

namespace EmployeeSystem.Services
{
    public class DbEmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IEmployeeMapper _mapper;
        private readonly ILogger<DbEmployeeService> _logger;

        public DbEmployeeService(IEmployeeRepository repo, IEmployeeMapper mapper, ILogger<DbEmployeeService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task AddAsync(EmployeeDto dto)
        {
            _logger.LogInformation("Adding employee");
            var employee = _mapper.ToEntity(dto);
            await _repo.AddAsync(employee);
            await _repo.SaveChangesAsync();
            _logger.LogInformation("Employee added");
        }

        public async Task UpdateAsync(EmployeeDto dto)
        {
            _logger.LogInformation("Updating employee");

            var employee = await _repo.GetByIdAsync(dto.EmployeeNumber);
            if (employee is null)
            {
                _logger.LogWarning("Employee not found");
                return;
            }

            _mapper.UpdateEntity(employee, dto);
            await _repo.UpdateAsync(employee);
            await _repo.SaveChangesAsync();

            _logger.LogInformation("Employee updated");
        }
    }
}
