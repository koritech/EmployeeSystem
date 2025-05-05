using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services.DTOs;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IEmployeeMapper
    {
        EmployeeDto ToDto(Employee entity);
        Employee ToEntity(EmployeeDto dto);
        void UpdateEntity(Employee entity, EmployeeDto dto);
    }
}
