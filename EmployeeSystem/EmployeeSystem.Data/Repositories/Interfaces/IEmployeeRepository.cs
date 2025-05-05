using EmployeeSystem.Domain.Entities;

namespace EmployeeSystem.Data.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync(string? nameFilter, int page, int pageSize);
        Task<Employee?> GetByIdAsync(int employeeNumber);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Employee employee);
        Task SaveChangesAsync();
    }
}
