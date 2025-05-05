using KafkaProducerService.Producer.Services;
using KafkaProducerService.Producer.Config;
using EmployeeService.Models;

var config = new KafkaConfig
{
    BootstrapServers = "localhost:9092",
    Topic = "employee-updates-test"
};

var producerService = new KafkaProducerService1(config);

var employee = new Employee
{
    EmployeeNumber = 105,
    Name = "Vivekananadan ramachandran",
    HourlyRate = 50,
    HoursWorked = 40
};

var count = 1;

while (true)
{
    employee.EmployeeNumber = count++;
    if (employee.EmployeeNumber == 200)
    {
        count = 1;
    }
    await producerService.ProduceAsync(employee);
    Task.Delay(1000).Wait();
}

