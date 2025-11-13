using NebuloHub.Domain.Enum;

namespace NebuloHub.Application.DTOs.Request
{
    public class CreateHabilidadeRequest
    {
        public string NomeHabilidade { get; set; }
        public string TipoHabilidade { get; set; }

    }
}
