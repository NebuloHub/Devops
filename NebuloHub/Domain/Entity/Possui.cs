using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NebuloHub.Domain.Entity
{
    public class Possui
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdPossui { get; set; }

        // Chave estrangeira
        public string StartupCNPJ { get; set; }
        public virtual Startup Startup { get; set; }

        public long IdHabilidade { get; set; }
        public virtual Habilidade Habilidade { get; set; }

        private Possui(string startupCNPJ, long idHabilidade)
        {
            StartupCNPJ = startupCNPJ;
            IdHabilidade = idHabilidade;
        }

        public void Atualizar(string startupCNPJ, long idHabilidade)
        {
            StartupCNPJ = startupCNPJ;
            IdHabilidade = idHabilidade;
        }


        internal static Possui Create(string startupCNPJ, long idHabilidade)
        {
            return new Possui(startupCNPJ, idHabilidade);
        }

        public Possui() { }
    }
}
