using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NebuloHub.Domain.Entity
{
    public class Startup
    {
        [Key]
        public string CNPJ { get; set; }
        public string? Video { get; set; }
        public string NomeStartup { get; set; }
        public string? Site { get; set; }
        public string Descricao { get; set; }
        public string? NomeResponsavel { get; set; }
        public string EmailStartup { get; set; }

        // Chave estrangeira
        public string UsuarioCPF { get; set; }
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<Possui> Possuis { get; private set; } = new List<Possui>();


        private Startup(string cnpj, string? video, string nomeStartup, string? site, string descricao, string? nomeResponsavel, string emailStartup, string usuarioCpf )
        {
            CNPJ = cnpj;
            Video = video;
            NomeStartup = nomeStartup;
            Site = site;
            Descricao = descricao;
            NomeResponsavel = nomeResponsavel;
            EmailStartup = emailStartup;
            UsuarioCPF = usuarioCpf;
        }

        public void Atualizar(string cnpj, string? video, string nomeStartup, string? site, string descricao, string? nomeResponsavel, string emailStartup, string usuarioCpf)
        {
            CNPJ = cnpj;
            Video = video;
            NomeStartup = nomeStartup;
            Site = site;
            Descricao = descricao;
            NomeResponsavel = nomeResponsavel;
            EmailStartup = emailStartup;
            UsuarioCPF = usuarioCpf;
        }


        internal static Startup Create(string cnpj, string? video, string nomeStartup, string? site, string descricao, string? nomeResponsavel, string emailStartup, string usuarioCpf)
        {
            return new Startup(cnpj, video, nomeStartup, site, descricao, nomeResponsavel, emailStartup, usuarioCpf);
        }

        public Startup() { }
    }
}