using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NebuloHub.Domain.Entity;

namespace NebuloHub.Infraestructure.Mapping
{
    public class StartupMapping : IEntityTypeConfiguration<Startup>
    {
        public void Configure(EntityTypeBuilder<Startup> builder)
        {
            builder
                .ToTable("STARTUP");

            builder
                .HasKey(p => p.CNPJ);

            builder
                .Property(p => p.CNPJ)
                .HasColumnName("CNPJ")
                .IsRequired();

            builder
                .Property(p => p.Video)
                .HasColumnName("VIDEO");

            builder
                .Property(p => p.NomeStartup)
                .IsRequired()
                .HasColumnName("NOME_STARTUP");

            builder
                .Property(p => p.Site)
                .HasColumnName("SITE");

            builder
                .Property(p => p.Descricao)
                .IsRequired()
                .HasColumnName("DESCRICAO");

            builder
                .Property(p => p.NomeResponsavel)
                .HasColumnName("NOME_RESPONSAVEL");

            builder
                .Property(p => p.EmailStartup)
                .IsRequired()
                .HasColumnName("EMAIL_STARTUP");

            builder
                .Property(p => p.UsuarioCPF)
                .HasColumnName("CPF")
                .IsRequired();

        }
    }
}
