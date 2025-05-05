using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeSystem.Services.DTOs;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IEmployeeQueryService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync(string? nameFilter, int page, int pageSize);
        Task<EmployeeDto?> GetByNumberAsync(int employeeNumber);
        Task DeleteAsync(int employeeNumber);
    }
}
