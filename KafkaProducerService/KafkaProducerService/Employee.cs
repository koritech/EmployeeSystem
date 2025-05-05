namespace EmployeeService.Models
{
    public class Employee
    {
        public int EmployeeNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public double HoursWorked { get; set; }

        // Computed property
        public decimal TotalPay => HourlyRate * (decimal)HoursWorked;
    }
}
