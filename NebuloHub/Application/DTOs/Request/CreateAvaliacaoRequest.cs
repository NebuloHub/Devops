namespace NebuloHub.Application.DTOs.Request
{
    public class CreateAvaliacaoRequest
    {
        public long Nota { get; set; }
        public string? Comentario { get; set; }


        // Relacionamento
        public string UsuarioCPF { get; set; }
        public string StartupCNPJ { get; set; }
    }
}
