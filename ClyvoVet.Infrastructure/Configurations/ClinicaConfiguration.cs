using ClyvoVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoVet.Infrastructure.Configurations;

public class ClinicaMapping : IEntityTypeConfiguration<Clinica>
{
    public void Configure(EntityTypeBuilder<Clinica> builder)
    {
        builder.ToTable("CLINICA");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("ID_CLINICA").ValueGeneratedOnAdd();

        builder.Property(c => c.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Cnpj).HasColumnName("CNPJ").HasMaxLength(20).IsRequired();
        builder.Property(c => c.Logradouro).HasColumnName("LOGRADOURO").HasMaxLength(200);

        builder.HasIndex(c => c.Cnpj).IsUnique().HasDatabaseName("CLINICA_CNPJ_UK");
    }
}