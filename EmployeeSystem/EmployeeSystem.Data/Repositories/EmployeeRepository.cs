using EmployeeSystem.Data.Interfaces;
using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using EmployeeSystem.Data.Models;

namespace EmployeeSystem.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IAppDbContext _context;

        public EmployeeRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Employee> Data, int TotalCount)> GetAllWithCountAsync(string? nameFilter, int page, int pageSize)
        {
            var query = _context.Employees.Include(e => e.WorkRecord).AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
                query = query.Where(e => e.Name.Contains(nameFilter));

            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public Task<Employee?> GetByIdAsync(int employeeNumber)
        {
            return _context.Employees.Include(e => e.WorkRecord).FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
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
