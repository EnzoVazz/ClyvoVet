using ClyvoVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVet.Infrastructure.Context;

public class ClyvoVetContext : DbContext
{
    public ClyvoVetContext(DbContextOptions<ClyvoVetContext> options) : base(options) { }

    public DbSet<Tutor> Tutores { get; set; }
    public DbSet<Clinica> Clinicas { get; set; }
    public DbSet<Veterinario> Veterinarios { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Prontuario> Prontuarios { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<bool>().HaveConversion<int>();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClyvoVetContext).Assembly);
    }
}