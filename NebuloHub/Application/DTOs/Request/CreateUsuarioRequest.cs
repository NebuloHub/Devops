using NebuloHub.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace NebuloHub.Application.DTOs.Request
{
    public class CreateUsuarioRequest
    {
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }

        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }
        public string Telefone { get; set; }
    }
}
