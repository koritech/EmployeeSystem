using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Services.Mappers
{
    public class EmployeeMapper : IEmployeeMapper
    {
        public EmployeeDto ToDto(Employee e) => new()
        {
            EmployeeNumber = e.EmployeeNumber,
            Name = e.Name,
            HourlyRate = e.HourlyRate,
            HoursWorked = e.HoursWorked
        };

        public Employee ToEntity(EmployeeDto dto) => new()
        {
            EmployeeNumber = dto.EmployeeNumber,
            Name = dto.Name,
            HourlyRate = dto.HourlyRate,
            HoursWorked = dto.HoursWorked
        };

        public void UpdateEntity(Employee entity, EmployeeDto dto)
        {
            entity.Name = dto.Name;
            entity.HourlyRate = dto.HourlyRate;
            entity.HoursWorked = dto.HoursWorked;
        }
    }

}
