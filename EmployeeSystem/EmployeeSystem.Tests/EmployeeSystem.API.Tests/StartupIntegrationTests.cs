using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace EmployeeSystem.Tests.API
{
    public class StartupIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        [Test]
        public async Task SwaggerEndpoint_ShouldReturnSuccess()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/swagger/index.html");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        //Fix this
        //[Test]
        //public async Task GetAllEmployees_ShouldReturnOk()
        //{
        //    var client = _factory.CreateClient();
        //    client.DefaultRequestHeaders.Add("X-Processing-Mode", "db");

        //    var response = await client.GetAsync("/api/employee?page=1&pageSize=10");

        //    var content = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine($"Status: {response.StatusCode}");
        //    Console.WriteLine($"Response: {content}");

        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        //}

    }
}
