using EmployeeSystem.Data.Interfaces;
using EmployeeSystem.Data.Models;
using EmployeeSystem.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeSystem.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(IAppDbContext context, ILogger<EmployeeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(IEnumerable<Employee> Data, int TotalCount)> GetAllWithCountAsync(string? nameFilter, int page, int pageSize)
        {
            _logger.LogInformation("Fetching employees with pagination");

            var query = _context.Employees.Include(e => e.WorkRecord).AsQueryable();

            if (!string.IsNullOrEmpty(nameFilter))
            {
                _logger.LogInformation("Applying name filter");
                query = query.Where(e => e.Name.Contains(nameFilter));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Employees fetched");

            return (data, totalCount);
        }

        public Task<Employee?> GetByIdAsync(int employeeNumber)
        {
            _logger.LogInformation("Fetching employee by ID");
            return _context.Employees.Include(e => e.WorkRecord).FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
        }

        public async Task AddAsync(Employee employee)
        {
            _logger.LogInformation("Adding new employee");
            await _context.Employees.AddAsync(employee);
        }

        public Task UpdateAsync(Employee employee)
        {
            _logger.LogInformation("Updating employee");
            _context.Employees.Update(employee);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Employee employee)
        {
            _logger.LogInformation("Deleting employee");
            _context.Employees.Remove(employee);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
        {
            _logger.LogInformation("Saving changes to database");
            return _context.SaveChangesAsync();
        }
    }
}
