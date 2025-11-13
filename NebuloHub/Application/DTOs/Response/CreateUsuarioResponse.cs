using NebuloHub.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace NebuloHub.Application.DTOs.Response
{
    public class CreateUsuarioResponse
    {
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }

        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }
        public long? Telefone { get; set; }
    }
}
