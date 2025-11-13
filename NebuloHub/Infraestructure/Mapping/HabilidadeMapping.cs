using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NebuloHub.Domain.Entity;

namespace NebuloHub.Infraestructure.Mapping
{
    public class HabilidadeMapping : IEntityTypeConfiguration<Habilidade>
    {
        public void Configure(EntityTypeBuilder<Habilidade> builder)
        {
            builder
                .ToTable("HABILIDADE");

            builder
                .HasKey(s => s.IdHabilidade);

            builder
                .Property(s => s.IdHabilidade)
                .HasColumnName("ID_HABILIDADE")
                .HasDefaultValueSql("HABILIDADE_SEQ.NEXTVAL")
                .IsRequired();

            builder
                .Property(s => s.NomeHabilidade)
                .HasColumnName("NOME_HABILIDADE")
                .IsRequired();

            builder
                .Property(s => s.TipoHabilidade)
                .HasColumnName("TIPO_HABILIDADE")
                .IsRequired();

        }
    }
}
