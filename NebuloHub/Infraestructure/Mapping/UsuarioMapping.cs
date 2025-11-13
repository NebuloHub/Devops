using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NebuloHub.Domain.Entity;

namespace NebuloHub.Infraestructure.Mapping
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder
                .ToTable("USUARIO");

            builder
                .HasKey(s => s.CPF);

            builder
                .Property(s => s.CPF)
                .HasColumnName("CPF")
                .IsRequired();

            builder
                .Property(s => s.Nome)
                .HasColumnName("NOME")
                .IsRequired();

            builder
                .Property(s => s.Email)
                .HasColumnName("EMAIL")
                .IsRequired();

            builder
                .Property(s => s.Senha)
                .HasColumnName("SENHA")
                .IsRequired();

            builder
                .Property(s => s.Role)
                .HasColumnName("ROLE")
                .HasConversion<string>();

            builder
                .Property(s => s.Telefone)
                .HasColumnName("TELEFONE");
        }
    }
}
