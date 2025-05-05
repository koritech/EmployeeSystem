using EmployeeSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.IO;

namespace EmployeeSystem.Tests.Data
{
    [TestFixture]
    public class DbContextFactoryTests
    {
        private string _tempFolder;
        private string _appSettingsPath;

        [SetUp]
        public void Setup()
        {
            // Create a temp folder and simulate an appsettings.json file
            _tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempFolder);

            _appSettingsPath = Path.Combine(_tempFolder, "appsettings.json");
            File.WriteAllText(_appSettingsPath,
            @"{
                ""ConnectionStrings"": {
                    ""DefaultConnection"": ""Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;""
                }
            }");
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(_tempFolder))
                Directory.Delete(_tempFolder, recursive: true);
        }

        [Test]
        public void CreateDbContext_WithValidConfig_ShouldReturnContext()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .SetBasePath(_tempFolder)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Act
            var context = new AppDbContext(optionsBuilder.Options);

            // Assert
            Assert.IsNotNull(context);
            Assert.IsInstanceOf<AppDbContext>(context);
        }

        [Test]
        public void CreateDbContext_WithMissingConnection_ShouldThrow()
        {
            // Arrange: Delete the file so config will fail
            File.Delete(_appSettingsPath);

            var ex = Assert.Throws<FileNotFoundException>(() =>
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(_tempFolder)
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();
            });

            Assert.That(ex.Message, Does.Contain("appsettings.json"));
        }
    }
}
