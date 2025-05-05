using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Models;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IEmployeeQueryService
    {
        Task<EmployeePagedResult> GetAllEmployeesPagedAsync(string? nameFilter, int page, int pageSize);
        Task<EmployeeDto?> GetByEmployeeNumberAsync(int employeeNumber);
        Task DeleteEmployeeAsync(int employeeNumber);
    }
}
