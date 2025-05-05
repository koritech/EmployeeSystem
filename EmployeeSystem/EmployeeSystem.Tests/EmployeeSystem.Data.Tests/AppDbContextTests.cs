using EmployeeSystem.Data;
using EmployeeSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Tests.Data
{
    [TestFixture]
    public class AppDbContextTests
    {
        private AppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new AppDbContext(options);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void CanCreateContext_AndAccessEmployeesDbSet()
        {
            Assert.NotNull(_context.Employees);
        }

        [Test]
        public async Task CanAddEmployee_AndRetrieveByPrimaryKey()
        {
            var employee = new Employee
            {
                EmployeeNumber = 101,
                Name = "Vivek Ramachandran",
                HourlyRate = 50,
                HoursWorked = 40
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            var result = await _context.Employees.FindAsync(101);

            Assert.NotNull(result);
            Assert.AreEqual("Vivek Ramachandran", result.Name);
        }

        [Test]
        public async Task PrimaryKey_EmployeeNumber_ShouldPreventDuplicates()
        {
            var emp1 = new Employee { EmployeeNumber = 1, Name = "Test", HourlyRate = 20, HoursWorked = 10 };
            var emp2 = new Employee { EmployeeNumber = 1, Name = "Duplicate", HourlyRate = 25, HoursWorked = 15 };

            await _context.Employees.AddAsync(emp1);
            await _context.SaveChangesAsync();

            // Create a new context (simulate a second insert attempt elsewhere)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb") // use same db
                .Options;

            using var newContext = new AppDbContext(options);
            await newContext.Employees.AddAsync(emp2);

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await newContext.SaveChangesAsync();
            });
        }
    }
}
