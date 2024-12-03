using Common.Application;
using Common.Infrastructure;
using Common.Infrastructure.Configuration;
using Common.Presentation.Endpoints;
using HealthChecks.UI.Client;
using Inventory.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Starter.API.Extensions;
using Starter.API.Middlewares;
using Starter.API.OpenTelemetry;
using System.Infrastructure;
using System.Reflection;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Controller API Support
builder.Services.AddControllers().AddJsonOptions(x =>
     x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

//Serilog
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

//Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//Minimal API Support
builder.Services.AddEndpointsApiExplorer();

//Application Module Assemblies
Assembly[] moduleApplicationAssemblies =
[
    System.Application.AssemblyReference.Assembly,
    Inventory.Application.AssemblyReference.Assembly
];

// Adding Common Application Module
builder.Services.AddCommonApplication(moduleApplicationAssemblies);

// Adding Common Infrastructure Module
//string systemDatabase = builder.Configuration.GetValueOrThrow<string>("Database:DefaultConnection");
string redisConnectionString = builder.Configuration.GetValueOrThrow<string>("Redis:DefaultConnection");
string rabbitmqConnectionString = builder.Configuration.GetValueOrThrow<string>("RabbitMQ:DefaultConnection");
string postgresConnectionString = builder.Configuration.GetValueOrThrow<string>("Postgres:DefaultConnection");

builder.Services.AddCommonInfrastructure(
    DiagnosticsConfig.ServiceName,
    [
        SystemModule.ConfigureConsumers,
        //InventoryModule.ConfigureConsumers(redisConnectionString)
    ],
    redisConnectionString,
    rabbitmqConnectionString,
    postgresConnectionString);

//Adding Other Modules
builder.Services.AddSystemModule(builder.Configuration);
builder.Services.AddInventoryModule(builder.Configuration);

//Module Configurations
builder.Configuration.AddModuleConfiguration(["system", "inventory"]);

//API Documentation
builder.Services.AddOpenApi();

//Health Checks
builder.Services.AddHealthChecks();
//.AddSqlServer(testDatabase!)
//.AddRedis(redisConnectionString);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        // Allow all origins (adjust as necessary)
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSingleton<DatabaseInitializer>();

WebApplication app = builder.Build();

app.UseCors("MyPolicy");

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Starter.API"));

    await app.ApplyMigrations();


}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();

app.UseLogContextTraceLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapEndpoints();

await app.RunAsync();


//public partial class Program;
//This is how to access strongly-typed configurations in Program.cs
//var weatherOptions = builder.Configuration.GetSection(nameof(WeatherOptions)).Get<WeatherOptions>();
