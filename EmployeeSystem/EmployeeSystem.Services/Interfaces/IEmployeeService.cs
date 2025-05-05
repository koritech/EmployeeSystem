using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeSystem.Services.DTOs;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync(string? nameFilter, int page, int pageSize);
        Task<EmployeeDto?> GetByNumberAsync(int employeeNumber);
        Task AddAsync(EmployeeDto employee);
        Task UpdateAsync(EmployeeDto employee);
        Task DeleteAsync(int employeeNumber);
    }
}
