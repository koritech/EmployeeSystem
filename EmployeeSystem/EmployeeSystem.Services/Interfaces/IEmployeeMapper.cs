﻿using EmployeeSystem.Data.Models;
using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services.DTOs;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IEmployeeMapper
    {
        EmployeeResponseDto ToDto(Employee entity);
        Employee ToEntity(EmployeeDto dto);
        void UpdateEntity(Employee entity, EmployeeDto dto);
    }
}
