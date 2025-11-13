using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NebuloHub.Domain.Entity;

namespace NebuloHub.Infraestructure.Mapping
{
    public class AvaliacaoMapping : IEntityTypeConfiguration<Avaliacao>
    {
        public void Configure(EntityTypeBuilder<Avaliacao> builder)
        {
            builder
                .ToTable("AVALIACAO");

            builder
                .HasKey(p => p.IdAvaliacao);

            builder
                .Property(p => p.IdAvaliacao)
                .HasColumnName("ID_AVALIACAO")
                .HasDefaultValueSql("AVALIACAO_SEQ.NEXTVAL")
                .IsRequired();

            builder
                .Property(p => p.Nota)
                .HasColumnName("NOTA");

            builder
                .Property(p => p.Comentario)
                .HasColumnName("COMENTARIO");

            builder
                .Property(p => p.UsuarioCPF)
                .HasColumnName("CPF")
                .IsRequired();

            builder
                .Property(p => p.StartupCNPJ)
                .HasColumnName("CNPJ")
                .IsRequired();

        }
    }
}
