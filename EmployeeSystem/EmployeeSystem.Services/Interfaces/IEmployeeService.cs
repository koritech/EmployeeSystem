﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeSystem.Services.DTOs;

namespace EmployeeSystem.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task AddAsync(EmployeeDto employee);
        Task UpdateAsync(EmployeeDto employee);
    }
}
