using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NebuloHub.Domain.Entity
{
    public class Avaliacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdAvaliacao { get; set; }

        public long Nota { get; set; }
        public string? Comentario { get; set; }

        // Chave estrangeira
        public string UsuarioCPF { get; set; }
        [ForeignKey("UsuarioCPF")]
        public virtual Usuario Usuario { get; set; }

        public string StartupCNPJ { get; set; }
        public virtual Startup Startup { get; set; }

        private Avaliacao(long nota, string comentario, string usuarioCPF, string startupCNPJ)
        {
            Nota = nota;
            Comentario = comentario;
            UsuarioCPF = usuarioCPF;
            StartupCNPJ = startupCNPJ;
        }

        public void Atualizar(long nota, string comentario, string usuarioCPF, string startupCNPJ)
        {
            Nota = nota;
            Comentario = comentario;
            UsuarioCPF = usuarioCPF;
            StartupCNPJ = startupCNPJ;
        }


        internal static Avaliacao Create(long nota, string comentario, string usuarioCPF, string startupCNPJ)
        {
            return new Avaliacao(nota, comentario, usuarioCPF, startupCNPJ);
        }

        public Avaliacao() { }
    }
}