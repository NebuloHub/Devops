using NebuloHub.Domain.Enum;

namespace NebuloHub.Application.DTOs.Request
{
    public class CreateStartupRequest
    {
        public string CNPJ { get; set; }
        public string? Video { get; set; }
        public string NomeStartup { get; set; }
        public string? Site { get; set; }
        public string Descricao { get; set; }
        public string? NomeResponsavel { get; set; }
        public string EmailStartup { get; set; }

        // Relacionamento
        public string UsuarioCPF { get; set; }
    }
}
