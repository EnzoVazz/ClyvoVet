using ClyvoVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoVet.Infrastructure.Configurations;

public class TutorMapping : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("TUTOR");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("ID_TUTOR").ValueGeneratedOnAdd();

        builder.Property(t => t.Nome).HasColumnName("NOME").HasMaxLength(100).IsRequired();
        builder.Property(t => t.Cpf).HasColumnName("CPF").HasMaxLength(14).IsRequired();
        builder.Property(t => t.Telefone).HasColumnName("TELEFONE").HasMaxLength(20);

        builder.HasIndex(t => t.Cpf).IsUnique().HasDatabaseName("TUTOR_CPF_UK");
        
        builder.HasMany(t => t.Pets).WithOne(p => p.Tutor).HasForeignKey(p => p.IdTutor);
    }
}