using ClyvoVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoVet.Infrastructure.Configurations;

public class ConsultaMapping : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.ToTable("CONSULTA");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("ID_CONSULTA").ValueGeneratedOnAdd();

        builder.Property(c => c.IdVeterinario).HasColumnName("ID_VETERINARIO").IsRequired();
        builder.Property(c => c.IdPet).HasColumnName("ID_PET").IsRequired();
        builder.Property(c => c.IdClinica).HasColumnName("ID_CLINICA").IsRequired();
        builder.Property(c => c.IdProntuario).HasColumnName("ID_PRONTUARIO");
        
        builder.Property(c => c.DataConsulta).HasColumnName("DATA_CONSULTA").IsRequired();
        builder.Property(c => c.Status).HasColumnName("STATUS").HasMaxLength(20);

        // Chaves Estrangeiras
        builder.HasOne(c => c.Veterinario).WithMany().HasForeignKey(c => c.IdVeterinario);
        builder.HasOne(c => c.Pet).WithMany(p => p.Consultas).HasForeignKey(c => c.IdPet);
        builder.HasOne(c => c.Clinica).WithMany().HasForeignKey(c => c.IdClinica);
        builder.HasOne(c => c.Prontuario).WithMany().HasForeignKey(c => c.IdProntuario);
    }
}