using EmployeeSystem.Services.Models;

namespace EmployeeSystem.Services.Validation
{
    public interface IRequestValidator
    {
        ValidationResult Validate<T>(T model);
        ValidationResult ValidateNumber(int number, string fieldName);
    }
}
