using System.Text;
using ClyvoVet.Api.Auth;
using ClyvoVet.Api.Hateoas;
using ClyvoVet.Api.HealthChecks;
using ClyvoVet.Api.Middleware;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Infrastructure;
using ClyvoVet.Infrastructure.Context;
using ClyvoVet.Infrastructure.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;

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
    var testing = builder.Environment.IsEnvironment("Testing");

    builder.Host.UseSerilog();

    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
        ?? throw new InvalidOperationException("Configuração Jwt ausente.");

    if (string.IsNullOrWhiteSpace(jwtSettings.Key) || Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
        throw new InvalidOperationException("A chave JWT precisa ter pelo menos 32 bytes.");

    if (testing)
    {
        builder.Services.AddDbContext<ClyvoVetContext>(options =>
            options.UseInMemoryDatabase("ClyvoVetTests"));
    }
    else
    {
        builder.Services.AddDbContext<ClyvoVetContext>(options =>
            options.UseOracle(
                builder.Configuration.GetConnectionString("OracleConnection"),
                oracleOptions => oracleOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)));
    }

    builder.Services.AddInfrastructure(builder.Configuration, testing);
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
    builder.Services.Configure<AuthUsersOptions>(builder.Configuration.GetSection("AuthUsers"));
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IUsuarioAtual, UsuarioAtual>();
    builder.Services.AddScoped<TokenService>();
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<HateoasFactory>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                NameClaimType = System.Security.Claims.ClaimTypes.Name,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });

    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter())
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter());

    var healthChecks = builder.Services.AddHealthChecks();
    if (testing)
    {
        healthChecks.AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Ambiente de teste."));
    }
    else
    {
        var oracleConnection = builder.Configuration.GetConnectionString("OracleConnection")
            ?? throw new InvalidOperationException("Connection string OracleConnection ausente.");

        healthChecks
            .AddOracle(oracleConnection, name: "Database_Oracle")
            .AddCheck<MongoHealthCheck>("Database_Mongo")
            .AddUrlGroup(new Uri("https://www.fiap.com.br"), name: "FIAP")
            .AddUrlGroup(new Uri("https://www.google.com"), name: "GOOGLE");
    }

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Clyvo Vet API",
            Version = "v1",
            Description = "Backend de medicina veterinária digital. JWT, HATEOAS, paginação e auditoria em MongoDB."
        });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Informe apenas o token JWT retornado por POST /api/auth/login."
        });
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
    });

    var app = builder.Build();

    app.UseGlobalExceptionHandling();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
    }).AllowAnonymous();

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
