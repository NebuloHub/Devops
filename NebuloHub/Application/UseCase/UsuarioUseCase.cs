using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Repositores;

namespace NebuloHub.Application.UseCase
{
    public class UsuarioUseCase
    {
        private readonly IRepository<Usuario> _repository;

        public UsuarioUseCase(IRepository<Usuario> repository)
        {
            _repository = repository;
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

        public async Task<List<CreateUsuarioResponse>> GetAllUsuarioAsync()
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

        /// <summary>
        /// Retorna os Usuario paginados.
        /// </summary>
        public async Task<List<CreateUsuarioResponse>> GetAllPagedAsync(int page, int pageSize)
        {
            var usuario = await _repository.GetAllAsync();

            var paged = usuario
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new CreateUsuarioResponse
                {
                    CPF = u.CPF,
                    Nome = u.Nome,
                    Email = u.Email,
                    Senha = u.Senha,
                    Role = u.Role,
                    Telefone = u.Telefone
                })
                .ToList();

            return paged;
        }

        public async Task<CreateUsuarioResponse?> GetByIdAsync(string cpf)
        {
            var usuario = await _repository.GetByIdAsync(cpf);
            if (usuario == null) return null;

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

        public async Task<bool> UpdateUsuarioAsync(string cpf, CreateUsuarioRequest request)
        {
            var usuario = await _repository.GetByIdAsync(cpf);
            if (usuario == null) return false;

            usuario.Atualizar(
                request.CPF,
                request.Nome,
                request.Email,
                request.Senha,
                request.Role,
                request.Telefone
            );
            _repository.Update(usuario);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUsuarioAsync(string cpf)
        {
            var usuario = await _repository.GetByIdAsync(cpf);
            if (usuario == null) return false;

            _repository.Delete(usuario);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
