using Application.Services;
using EmployeeConsumerService.Domain.Interfaces;
using EmployeeSystem.Data;
using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Data.Repositories;
using EmployeeSystem.Services;
using EmployeeSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using EmployeeSystem.Data.Interfaces;
using EmployeeSystem.Services.Mappers;
using EmployeeSystem.Services.Services;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        //Register EF Core using appsettings.json
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                context.Configuration.GetConnectionString("DefaultConnection")));

        //Register application services
        services.AddScoped<IEmployeeService, DbEmployeeService>();
        services.AddScoped<IEmployeeQueryService, EmployeeQueryService>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeMapper, EmployeeMapper>();
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        //Register Kafka consumer and background worker
        services.AddScoped<IKafkaConsumerService, KafkaConsumerService>();
        services.AddHostedService<KafkaHostedWorker>();
    })
    .Build()
    .Run();