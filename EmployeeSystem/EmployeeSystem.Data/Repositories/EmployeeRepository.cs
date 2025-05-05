using EmployeeSystem.Data.Interfaces;
using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IAppDbContext _context;

        public EmployeeRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(string? nameFilter, int page, int pageSize)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
                query = query.Where(e => e.Name.Contains(nameFilter));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<Employee?> GetByIdAsync(int employeeNumber)
        {
            return _context.Employees.FindAsync(employeeNumber).AsTask();
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
