using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Models;
using EmployeeSystem.Services.Validation;

namespace EmployeeSystem.Services.Validator
{

    public class RequestValidator : IRequestValidator
    {
        public ValidationResult ValidateNumber(int number, string fieldName)
        {
            var result = new ValidationResult();
            if (number <= 0)
                result.Errors.Add($"{fieldName} must be greater than 0.");
            return result;
        }

        public ValidationResult Validate<T>(T model)
        {
            var result = new ValidationResult();

            if (model is EmployeeDto dto)
            {
                if (dto.EmployeeNumber <= 0)
                    result.Errors.Add("EmployeeNumber must be greater than 0.");

                if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length < 2)
                    result.Errors.Add("Name must be at least 2 characters.");

                if (dto.HourlyRate < 0)
                    result.Errors.Add("HourlyRate cannot be negative.");

                if (dto.HoursWorked < 0)
                    result.Errors.Add("HoursWorked cannot be negative.");
            }

            return result;
        }
    }
}

