using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSystem.Services.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public decimal HoursWorked { get; set; }
    }
}
