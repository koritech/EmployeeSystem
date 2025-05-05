// In EmployeeSystem.Data/Interfaces/IAppDbContext.cs
using EmployeeSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Data.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Employee> Employees { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
