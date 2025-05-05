using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSystem.Data.Models
{
    public class EmployeeWorkRecord
    {
        [Key]
        [ForeignKey("Employee")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeNumber { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? HoursWorked { get; set; }

        public Employee Employee { get; set; } = null!;

    }
}
