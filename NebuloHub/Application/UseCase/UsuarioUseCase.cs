using Microsoft.EntityFrameworkCore;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;

namespace NebuloHub.Application.UseCase
{
    public class UsuarioUseCase
    {
        private readonly IRepository<Usuario> _repository;
        private readonly AppDbContext _context;

        public UsuarioUseCase(IRepository<Usuario> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CreateUsuarioResponse> CreateUsuarioAsync(CreateUsuarioRequest request)
        {
            var usuario = Usuario.Create(
                request.CPF,
                request.Nome,
                request.Email,
                request.Senha,
                request.Role,
                request.Telefone
            );

            await _repository.AddAsync(usuario);
            await _repository.SaveChangesAsync();

            return new CreateUsuarioResponse
            {
                CPF = usuario.CPF,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Senha = usuario.Senha,
                Role = usuario.Role,
                Telefone = usuario.Telefone
            };
        }

        public async Task<List<CreateUsuarioResponse>> GetAllPagedAsync()
        {
            var usuario = await _repository.GetAllAsync();

            return usuario.Select(u => new CreateUsuarioResponse
            {
                CPF = u.CPF,
                Nome = u.Nome,
                Email = u.Email,
                Senha = u.Senha,
                Role = u.Role,
                Telefone = u.Telefone
            }).ToList();
        }

        public async Task<CreateUsuarioResponse?> GetByIdAsync(string cpf)
        {
            var usuario = await _context.Usuario
                .Include(u => u.Startups)
                .FirstOrDefaultAsync(u => u.CPF == cpf);

            if (usuario == null) return null;

            return new CreateUsuarioResponse
            {
                CPF = usuario.CPF,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Senha = usuario.Senha,
                Role = usuario.Role,
                Telefone = usuario.Telefone,

                Startups = usuario.Startups.Select(s => new CreateStartupResponse
                {
                    CNPJ = s.CNPJ,
                    Video = s.Video,
                    NomeStartup = s.NomeStartup,
                    Site = s.Site,
                    Descricao = s.Descricao,
                    NomeResponsavel = s.NomeResponsavel,
                    EmailStartup = s.EmailStartup,
                    UsuarioCPF = s.UsuarioCPF
                }).ToList()
            };
        }


        public async Task<CreateUsuarioResponse> UpdateUsuarioAsync(string cpf, CreateUsuarioRequest request)
        {
            var usuario = await _repository.GetByIdAsync(cpf);
            if (usuario == null) return null;

            usuario.Atualizar(
                request.Nome,
                request.Email,
                request.Senha,
                request.Role,
                request.Telefone
            );
            _repository.Update(usuario);
            await _repository.SaveChangesAsync();


            return new CreateUsuarioResponse
            {
                CPF = usuario.CPF,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Senha = usuario.Senha,
                Role = usuario.Role,
                Telefone = usuario.Telefone
            };
        }

        public async Task<bool> DeleteUsuarioAsync(string cpf)
        {
            var usuario = await _repository.GetByIdAsync(cpf);
            if (usuario == null) return false;

            _repository.Delete(usuario);
            await _repository.SaveChangesAsync();
            return true;
        }

        // Método de login
        public async Task<Usuario?> LoginAsync(string email, string senha)
        {
            // Busca o usuário por e-mail e senha
            var usuarios = await _repository.GetAllAsync();
            var usuario = usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);

            return usuario;
        }
    }
}
