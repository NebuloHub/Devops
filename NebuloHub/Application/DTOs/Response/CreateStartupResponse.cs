namespace NebuloHub.Application.DTOs.Response
{
    public class CreateStartupResponse
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

        public List<CreateAvaliacaoResponse> Avaliacoes { get; set; }
    }
}
