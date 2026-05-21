using ClyvoVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoVet.Infrastructure.Configurations;

public class VeterinarioMapping : IEntityTypeConfiguration<Veterinario>
{
    public void Configure(EntityTypeBuilder<Veterinario> builder)
    {
        builder.ToTable("VETERINARIO");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasColumnName("ID_VETERINARIO").ValueGeneratedOnAdd();

        builder.Property(v => v.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
        builder.Property(v => v.Crmv).HasColumnName("CRMV").HasMaxLength(20).IsRequired();
        builder.Property(v => v.Telefone).HasColumnName("TELEFONE").HasMaxLength(20);

        builder.HasIndex(v => v.Crmv).IsUnique().HasDatabaseName("VETERINARIO_CRMV_UK");
    }
}