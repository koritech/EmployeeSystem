using EmployeeSystem.Data.Interfaces;
using EmployeeSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeWorkRecord> EmployeeWorkRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.WorkRecord)
                .WithOne(w => w.Employee)
                .HasForeignKey<EmployeeWorkRecord>(w => w.EmployeeNumber);

            modelBuilder.Entity<EmployeeWorkRecord>()
            .Property(w => w.HourlyRate)
            .HasPrecision(18, 2);

            modelBuilder.Entity<EmployeeWorkRecord>()
                .Property(w => w.HoursWorked)
                .HasPrecision(18, 2);
        }
    }
}
