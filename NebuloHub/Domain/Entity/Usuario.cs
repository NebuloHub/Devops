using NebuloHub.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NebuloHub.Domain.Entity
{
    public class Usuario
    {
        [Key]
        public string CPF { get; set; }

        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public Role Role { get; set; }
        public string? Telefone { get; set; }
        


        private Usuario(string cpf, string nome, string email, string senha, Role role, string telefone)
        {
            CPF = cpf;
            Nome = nome;
            Email = email;
            Senha = senha;
            Role = role;
            Telefone = telefone;

        }

        public void Atualizar(string cpf, string nome, string email, string senha, Role role, string telefone)
        {
            CPF = cpf;
            Nome = nome;
            Email = email;
            Senha = senha;
            Role = role;
            Telefone = telefone;
        }


        internal static Usuario Create(string cpf, string nome, string email, string senha, Role role, string telefone)
        {
            return new Usuario(cpf, nome, email, senha, role, telefone);
        }

        public Usuario() { }
    }

}