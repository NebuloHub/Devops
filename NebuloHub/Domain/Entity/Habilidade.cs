using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NebuloHub.Domain.Entity
{
    public class Habilidade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdHabilidade { get; set; }
        public string NomeHabilidade { get; set; }
        public string TipoHabilidade { get; set; }


        public virtual Possui Possui { get; set; }


        private Habilidade(string nomeHabilidade, string tipoHabilidade)
        {
            NomeHabilidade = nomeHabilidade;
            TipoHabilidade = tipoHabilidade;

        }

        public void Atualizar(string nomeHabilidade, string tipoHabilidade)
        {
            NomeHabilidade = nomeHabilidade;
            TipoHabilidade = tipoHabilidade;
        }


        internal static Habilidade Create(string nomeHabilidade, string tipoHabilidade)
        {
            return new Habilidade(nomeHabilidade, tipoHabilidade);
        }

        public Habilidade() { }
    }

}