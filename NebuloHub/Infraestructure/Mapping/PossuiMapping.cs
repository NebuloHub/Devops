using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NebuloHub.Domain.Entity;

namespace NebuloHub.Infraestructure.Mapping
{
    public class PossuiMapping : IEntityTypeConfiguration<Possui>
    {
        public void Configure(EntityTypeBuilder<Possui> builder)
        {
            builder
                .ToTable("POSSUI");

            builder
                .HasKey(p => p.IdPossui);

            builder
                .Property(p => p.IdPossui)
                .HasColumnName("ID_POSSUI")
                .HasDefaultValueSql("POSSUI_SEQ.NEXTVAL")
                .IsRequired();

            builder
                .Property(p => p.StartupCNPJ)
                .HasColumnName("CNPJ")
                .IsRequired();

            builder
                .Property(p => p.IdHabilidade)
                .HasColumnName("ID_HABILIDADE")
                .IsRequired();


            builder
                .HasOne(p => p.Startup)
                .WithMany(s => s.Possuis) 
                .HasForeignKey(p => p.StartupCNPJ)
                .HasPrincipalKey(s => s.CNPJ)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("CNPJ_POSSUI_FK");


            builder
                .HasOne(p => p.Habilidade)
                .WithMany(s => s.Possuis)
                .HasForeignKey(p => p.IdHabilidade)
                .HasPrincipalKey(h => h.IdHabilidade)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("ID_HABILIDADE_POSSUI_FK");
        }
    }
}
