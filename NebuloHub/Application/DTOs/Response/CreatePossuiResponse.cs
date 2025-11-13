namespace NebuloHub.Application.DTOs.Response
{
    public class CreatePossuiResponse
    {
        public long IdPossui { get; set; }

        // Relacionamento
        public CreateStartupResponse Startup { get; set; }
        public CreateHabilidadeResponse Habilidade { get; set; }
    }
}
