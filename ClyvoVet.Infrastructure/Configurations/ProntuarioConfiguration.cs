using ClyvoVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoVet.Infrastructure.Configurations;

public class ProntuarioMapping : IEntityTypeConfiguration<Prontuario>
{
    public void Configure(EntityTypeBuilder<Prontuario> builder)
    {
        builder.ToTable("PRONTUARIO");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("ID_PRONTUARIO").ValueGeneratedOnAdd();

        builder.Property(p => p.Diagnostico).HasColumnName("DIAGNOSTICO").HasMaxLength(500);
        builder.Property(p => p.DataRegistro).HasColumnName("DATA_REGISTRO");
    }
}