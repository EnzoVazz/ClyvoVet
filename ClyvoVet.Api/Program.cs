using ClyvoVet.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ClyvoVet.Api.HealthChecks;
using Serilog;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() 
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) 
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information) 
    .Enrich.FromLogContext() 
    .WriteTo.Console() 
    .WriteTo.File("logs/clyvovet-.txt", rollingInterval: RollingInterval.Day) 
    .CreateLogger();

try
{
    Log.Information("Iniciando a API ClyvoVet...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddDbContext<ClyvoVetContext>(options =>
        options.UseOracle(
            builder.Configuration.GetConnectionString("OracleConnection"),
            oracleOptions => oracleOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)
        ));

    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation() 
            .AddHttpClientInstrumentation() 
            .AddConsoleExporter())          
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation() 
            .AddConsoleExporter());         

    builder.Services.AddHealthChecks()
        .AddOracle(builder.Configuration.GetConnectionString("OracleConnection"), name: "Database_Oracle")
        .AddUrlGroup(new Uri("https://www.fiap.com.br"), name: "FIAP")
        .AddUrlGroup(new Uri("https://www.google.com"), name: "GOOGLE");

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations(); 
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging(); 

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao iniciar a API.");
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }