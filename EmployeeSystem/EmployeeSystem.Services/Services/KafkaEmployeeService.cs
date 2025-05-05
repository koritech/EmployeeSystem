using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Services
{
    public class KafkaEmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;

        public KafkaEmployeeService(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        public async Task AddAsync(EmployeeDto dto)
        {
            var employee = new Employee
            {
                EmployeeNumber = dto.EmployeeNumber,
                Name = dto.Name,
                HourlyRate = dto.HourlyRate,
                HoursWorked = dto.HoursWorked
            };

            await _repo.AddAsync(employee);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int employeeNumber)
        {
            var employee = await _repo.GetByIdAsync(employeeNumber);
            if (employee == null) return;

            await _repo.DeleteAsync(employee);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync(string? nameFilter, int page, int pageSize)
        {
            var list = await _repo.GetAllAsync(nameFilter, page, pageSize);

            return list.Select(e => new EmployeeDto
            {
                EmployeeNumber = e.EmployeeNumber,
                Name = e.Name,
                HourlyRate = e.HourlyRate,
                HoursWorked = e.HoursWorked
            });
        }

        public async Task<EmployeeDto?> GetByNumberAsync(int employeeNumber)
        {
            var e = await _repo.GetByIdAsync(employeeNumber);
            if (e == null) return null;

            return new EmployeeDto
            {
                EmployeeNumber = e.EmployeeNumber,
                Name = e.Name,
                HourlyRate = e.HourlyRate,
                HoursWorked = e.HoursWorked
            };
        }

        public async Task UpdateAsync(EmployeeDto dto)
        {
            var employee = await _repo.GetByIdAsync(dto.EmployeeNumber);
            if (employee == null) return;

            employee.Name = dto.Name;
            employee.HourlyRate = dto.HourlyRate;
            employee.HoursWorked = dto.HoursWorked;

            await _repo.UpdateAsync(employee);
            await _repo.SaveChangesAsync();
        }
    }
}
