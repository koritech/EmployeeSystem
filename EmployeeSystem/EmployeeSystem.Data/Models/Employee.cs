using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeSystem.Data.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public EmployeeWorkRecord? WorkRecord { get; set; }
    }
}
