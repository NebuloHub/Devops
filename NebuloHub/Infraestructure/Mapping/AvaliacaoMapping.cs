using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NebuloHub.Domain.Entity;

public class AvaliacaoMapping : IEntityTypeConfiguration<Avaliacao>
{
    public void Configure(EntityTypeBuilder<Avaliacao> builder)
    {
        builder.ToTable("AVALIACAO");

        builder.HasKey(p => p.IdAvaliacao);

        builder.Property(p => p.IdAvaliacao)
            .HasColumnName("ID_AVALIACAO")
            .IsRequired();

        builder.Property(p => p.Nota)
            .HasColumnName("NOTA");

        builder.Property(p => p.Comentario)
            .HasColumnName("COMENTARIO");



        builder.Property(a => a.UsuarioCPF)
            .HasColumnName("CPF")
            .IsRequired();

        builder.HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioCPF)
            .HasPrincipalKey(u => u.CPF)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(a => a.StartupCNPJ)
            .HasColumnName("STARTUP_CNPJ")
            .IsRequired();

        builder.HasOne(a => a.Startup)
            .WithMany(s => s.Avaliacoes)
            .HasForeignKey(a => a.StartupCNPJ)
            .HasPrincipalKey(s => s.CNPJ)
            .OnDelete(DeleteBehavior.Restrict);


    }
}
