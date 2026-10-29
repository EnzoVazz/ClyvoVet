using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Domain.Services;
using ClyvoVet.Infrastructure.Mongo;
using ClyvoVet.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace ClyvoVet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool testing)
    {
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IClinicaRepository, ClinicaRepository>();
        services.AddScoped<IVeterinarioRepository, VeterinarioRepository>();
        services.AddScoped<IProntuarioRepository, ProntuarioRepository>();
        services.AddScoped<IConsultaRepository, ConsultaRepository>();

        services.AddScoped<PetService>();
        services.AddScoped<TutorService>();
        services.AddScoped<ClinicaService>();
        services.AddScoped<VeterinarioService>();
        services.AddScoped<ProntuarioService>();
        services.AddScoped<ConsultaService>();
        services.AddScoped<AuditoriaService>();

        if (testing)
        {
            services.AddSingleton<IAuditoriaRepository, InMemoryAuditoriaRepository>();
            return services;
        }

        var connection = configuration.GetConnectionString("MongoConnection") ?? "mongodb://localhost:27017";
        var database = configuration["Mongo:Database"] ?? "clyvovet";
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connection));
        services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(database));
        services.AddSingleton<IAuditoriaRepository, MongoAuditoriaRepository>();
        return services;
    }
}
