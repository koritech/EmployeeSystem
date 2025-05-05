// In EmployeeSystem.Data/Interfaces/IAppDbContext.cs
using EmployeeSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Data.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeWorkRecord> EmployeeWorkRecords { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
