using Application.Services;
using EmployeeConsumerService.Domain.Interfaces;
using EmployeeSystem.Data;
using EmployeeSystem.Services;
using EmployeeSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        //Register EF Core using appsettings.json
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                context.Configuration.GetConnectionString("DefaultConnection")));

        //Register application services
        services.AddScoped<IEmployeeService, EmployeeSystemService>();

        //Register Kafka consumer and background worker
        services.AddScoped<IKafkaConsumerService, KafkaConsumerService>();
        services.AddHostedService<KafkaHostedWorker>();
    })
    .Build()
    .Run();