using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Factory
{
    public interface IEmployeeServiceFactory
    {
        IEmployeeService GetService(string mode);
    }
}
