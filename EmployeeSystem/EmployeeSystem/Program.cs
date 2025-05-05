using EmployeeSystem.Data;
using EmployeeSystem.Services.Interfaces;
using EmployeeSystem.Services;
using Microsoft.EntityFrameworkCore;
using EmployeeSystem.Data.Repositories.Interfaces;
using EmployeeSystem.Data.Repositories;
using EmployeeSystem.Data.Interfaces;
using EmployeeSystem.Factory;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using EmployeeSystem.Domain.Entities;
using EmployeeSystem.Services.Mappers;
using EmployeeSystem.Services.Validation;
using EmployeeSystem.Services.Validator;
using EmployeeSystem;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddLog4Net("log4net.config");
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Employee API", Version = "v1" });

            options.OperationFilter<AddRequiredHeaderParameter>();
        });
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        builder.Services.AddScoped<IRequestValidator, RequestValidator>();

        builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        builder.Services.AddScoped<IEmployeeMapper, EmployeeMapper>();
        builder.Services.AddScoped<DbEmployeeService>();
        builder.Services.AddScoped<KafkaEmployeeService>();
        builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
        builder.Services.AddSingleton<IProducer<string, string>>(sp =>
        {
            var kafkaSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;

            var config = new ProducerConfig
            {
                BootstrapServers = kafkaSettings.BootstrapServers,
                Acks = Enum.TryParse<Acks>(kafkaSettings.Acks, out var ack) ? ack : Acks.All,
                EnableIdempotence = kafkaSettings.EnableIdempotence
            };

            return new ProducerBuilder<string, string>(config).Build();
        });
        builder.Services.AddScoped<IEmployeeServiceFactory, EmployeeServiceFactory>();
        builder.Services.AddScoped<IEmployeeService>(provider =>
        {
            var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var mode = httpContext?.Request.Headers["X-Processing-Mode"].FirstOrDefault()?.ToLower() ?? "db";

            var factory = provider.GetRequiredService<IEmployeeServiceFactory>();
            return factory.GetService(mode); 
        });

        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<ProcessingModeValidationMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}