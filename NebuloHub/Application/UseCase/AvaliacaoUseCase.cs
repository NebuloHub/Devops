using Microsoft.EntityFrameworkCore;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;

namespace NebuloHub.Application.UseCase
{
    public class AvaliacaoUseCase
    {
        private readonly IRepository<Avaliacao> _repository;
        private readonly AppDbContext _context;


        public AvaliacaoUseCase(IRepository<Avaliacao> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CreateAvaliacaoResponse> CreateAvaliacaoAsync(CreateAvaliacaoRequest request)
        {
            var avaliacao = Avaliacao.Create(
                request.Nota,
                request.Comentario,
                request.UsuarioCPF,
                request.StartupCNPJ
            );

            await _repository.AddAsync(avaliacao);
            await _repository.SaveChangesAsync();

            return new CreateAvaliacaoResponse
            {
                IdAvaliacao = avaliacao.IdAvaliacao,
                Nota = avaliacao.Nota,
                Comentario = avaliacao.Comentario
            };
        }

        /// <summary>
        /// Retorna os Avaliacao paginados.
        /// </summary>
        public async Task<List<CreateAvaliacaoResponse>> GetAllPagedAsync(int page, int pageSize)
        {
            var avaliacao = await _repository.GetAllAsync();

            var paged = avaliacao
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new CreateAvaliacaoResponse
                {
                    IdAvaliacao = u.IdAvaliacao,
                    Nota = u.Nota,
                    Comentario = u.Comentario
                })
                .ToList();

            return paged;
        }

        public async Task<CreateAvaliacaoResponse?> GetByIdAsync(long id)
        {
            var avaliacao = await _context.Avaliacao
                .Include(m => m.Usuario)
                .Include(m => m.Startup)
                .FirstOrDefaultAsync(m => m.IdAvaliacao == id);

            if (avaliacao == null) return null;

            return new CreateAvaliacaoResponse
            {
                IdAvaliacao = avaliacao.IdAvaliacao,
                Nota = avaliacao.Nota,
                Comentario = avaliacao.Comentario,

                Usuario = avaliacao.Usuario == null ? null : new CreateUsuarioResponse
                {
                    CPF = avaliacao.Usuario.CPF,
                    Nome = avaliacao.Usuario.Nome,
                    Email = avaliacao.Usuario.Email,
                    Senha = avaliacao.Usuario.Senha,
                    Role = avaliacao.Usuario.Role,
                    Telefone = avaliacao.Usuario.Telefone
                },

                Startup = avaliacao.Startup == null ? null : new CreateStartupResponse
                {
                    CNPJ = avaliacao.Startup.CNPJ,
                    Video = avaliacao.Startup.Video,
                    NomeStartup = avaliacao.Startup.NomeStartup,
                    Site = avaliacao.Startup.Site,
                    Descricao = avaliacao.Startup.Descricao,
                    NomeResponsavel = avaliacao.Startup.NomeResponsavel
                }
            };
        }


        public async Task<bool> UpdateAvaliacaoAsync(long id, CreateAvaliacaoRequest request)
        {
            var avaliacao = await _repository.GetByIdAsync(id);
            if (avaliacao == null) return false;

            avaliacao.Atualizar(
                request.Nota,
                request.Comentario,
                request.UsuarioCPF,
                request.StartupCNPJ
            );
            _repository.Update(avaliacao);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAvaliacaoAsync(long id)
        {
            var avaliacao = await _repository.GetByIdAsync(id);
            if (avaliacao == null) return false;

            _repository.Delete(avaliacao);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
