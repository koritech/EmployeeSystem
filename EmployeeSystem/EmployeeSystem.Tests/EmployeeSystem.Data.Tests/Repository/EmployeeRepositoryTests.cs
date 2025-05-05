using EmployeeSystem.Data;
using EmployeeSystem.Data.Repositories;
using EmployeeSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSystem.Tests.Repositories
{
    [TestFixture]
    public class EmployeeRepositoryTests
    {
        private AppDbContext _context = null!;
        private EmployeeRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new AppDbContext(options);
            _repository = new EmployeeRepository(_context);

            _context.Employees.AddRange(
                new Employee { EmployeeNumber = 1, Name = "Vivek", HourlyRate = 20, HoursWorked = 10 },
                new Employee { EmployeeNumber = 2, Name = "John", HourlyRate = 22, HoursWorked = 8 },
                new Employee { EmployeeNumber = 3, Name = "Vishal", HourlyRate = 18, HoursWorked = 9 }
            );
            _context.SaveChanges();
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_WithFilter_ShouldReturnFilteredEmployees()
        {
            var result = await _repository.GetAllAsync("Vi", 1, 10);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllAsync_WithoutFilter_ShouldReturnAllEmployees()
        {
            var result = await _repository.GetAllAsync(null, 1, 10);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetByIdAsync_ExistingEmployee_ShouldReturnCorrect()
        {
            var result = await _repository.GetByIdAsync(2);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("John"));
        }

        [Test]
        public async Task AddAsync_ShouldAddEmployee()
        {
            var newEmp = new Employee { EmployeeNumber = 4, Name = "New Guy", HourlyRate = 30, HoursWorked = 5 };
            await _repository.AddAsync(newEmp);
            await _repository.SaveChangesAsync();

            var emp = await _context.Employees.FindAsync(4);
            Assert.That(emp, Is.Not.Null);
            Assert.That(emp!.Name, Is.EqualTo("New Guy"));
        }

        [Test]
        public async Task UpdateAsync_ShouldModifyEmployee()
        {
            var emp = await _repository.GetByIdAsync(1);
            emp!.Name = "Updated Name";

            await _repository.UpdateAsync(emp);
            await _repository.SaveChangesAsync();

            var updated = await _context.Employees.FindAsync(1);
            Assert.That(updated!.Name, Is.EqualTo("Updated Name"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveEmployee()
        {
            var emp = await _repository.GetByIdAsync(2);
            await _repository.DeleteAsync(emp!);
            await _repository.SaveChangesAsync();

            var deleted = await _context.Employees.FindAsync(2);
            Assert.That(deleted, Is.Null);
        }
    }
}
