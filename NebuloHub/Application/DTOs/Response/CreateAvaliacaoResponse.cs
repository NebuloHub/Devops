namespace NebuloHub.Application.DTOs.Response
{
    public class CreateAvaliacaoResponse
    {
        public long IdAvaliacao { get; set; }
        public long Nota { get; set; }
        public string? Comentario { get; set; }


        // Relacionamento
        public string UsuarioCPF { get; set; }
        public CreateUsuarioResponse Usuario { get; set; }
        public string StartupCNPJ { get; set; }
    }
}
