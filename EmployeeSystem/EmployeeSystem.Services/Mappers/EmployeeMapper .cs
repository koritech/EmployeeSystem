using EmployeeSystem.Data.Models;
using EmployeeSystem.Services.DTOs;
using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Services.Mappers
{
    public class EmployeeMapper : IEmployeeMapper
    {
        public EmployeeResponseDto ToDto(Employee e) => new()
        {
            EmployeeNumber = e.EmployeeNumber,
            Name = e.Name,
            HourlyRate = e.WorkRecord?.HourlyRate,
            HoursWorked = e.WorkRecord?.HoursWorked
        };

        public Employee ToEntity(EmployeeDto dto) => new()
        {
            EmployeeNumber = dto.EmployeeNumber,
            Name = dto.Name,
            WorkRecord = new EmployeeWorkRecord
            {
                EmployeeNumber = dto.EmployeeNumber,
                HourlyRate = dto.HourlyRate,
                HoursWorked = dto.HoursWorked
            }
        };

        public void UpdateEntity(Employee entity, EmployeeDto dto)
        {
            entity.Name = dto.Name;
            if (entity.WorkRecord != null)
            {
                entity.WorkRecord.HourlyRate = dto.HourlyRate;
                entity.WorkRecord.HoursWorked = dto.HoursWorked;
            }
        }
    }

}
