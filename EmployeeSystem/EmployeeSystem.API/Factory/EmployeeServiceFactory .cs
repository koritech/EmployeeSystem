using EmployeeSystem.Services;
using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Factory
{
    public class EmployeeServiceFactory : IEmployeeServiceFactory
    {
        private readonly IServiceProvider _provider;

        public EmployeeServiceFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IEmployeeService GetService(string mode) => mode switch
        {
            "kafka" => _provider.GetRequiredService<KafkaEmployeeService>(),
            "db" or _ => _provider.GetRequiredService<DbEmployeeService>()
        };
    }
}
