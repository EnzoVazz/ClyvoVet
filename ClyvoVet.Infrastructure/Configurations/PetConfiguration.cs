using ClyvoVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoVet.Infrastructure.Configurations;

public class PetMapping : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("PET");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("ID_PET").ValueGeneratedOnAdd();

        builder.Property(p => p.IdTutor).HasColumnName("ID_TUTOR").IsRequired();
        builder.Property(p => p.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
        builder.Property(p => p.Especie).HasColumnName("ESPECIE").HasMaxLength(50).IsRequired();
        builder.Property(p => p.Cor).HasColumnName("COR").HasMaxLength(30);
        builder.Property(p => p.Idade).HasColumnName("IDADE");
        builder.Property(p => p.Peso).HasColumnName("PESO").HasColumnType("NUMBER(5,2)");
    }
}