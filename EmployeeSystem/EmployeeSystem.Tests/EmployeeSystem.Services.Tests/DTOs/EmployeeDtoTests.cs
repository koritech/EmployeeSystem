using EmployeeSystem.Services.DTOs;
using NUnit.Framework;

namespace EmployeeSystem.Tests.EmployeeSystem.Services.Tests.DTOs
{
    [TestFixture]
    public class EmployeeDtoTests
    {
        [Test]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            var dto = new EmployeeDto();

            Assert.AreEqual(0, dto.EmployeeNumber);
            Assert.AreEqual(string.Empty, dto.Name);
            Assert.AreEqual(0m, dto.HourlyRate);
            Assert.AreEqual(0m, dto.HoursWorked);
        }

        [Test]
        public void Properties_ShouldStoreAssignedValues()
        {
            var dto = new EmployeeDto
            {
                EmployeeNumber = 1001,
                Name = "Vivek Ramachandran",
                HourlyRate = 75.50m,
                HoursWorked = 40
            };

            Assert.AreEqual(1001, dto.EmployeeNumber);
            Assert.AreEqual("Vivek Ramachandran", dto.Name);
            Assert.AreEqual(75.50m, dto.HourlyRate);
            Assert.AreEqual(40m, dto.HoursWorked);
        }
    }
}
