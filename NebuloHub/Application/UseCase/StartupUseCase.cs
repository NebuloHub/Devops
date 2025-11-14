using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;

namespace NebuloHub.Application.UseCase
{
    public class StartupUseCase
    {
        private readonly IRepository<Startup> _repository;
        private readonly AppDbContext _context;


        public StartupUseCase(IRepository<Startup> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CreateStartupResponse> CreateStartupAsync(CreateStartupRequest request)
        {
            var startup = Startup.Create(
                request.CNPJ,
                request.Video,
                request.NomeStartup,
                request.Site,
                request.Descricao,
                request.NomeResponsavel,
                request.EmailStartup,
                request.UsuarioCPF
            );

            await _repository.AddAsync(startup);
            await _repository.SaveChangesAsync();

            return new CreateStartupResponse
            {
                CNPJ = startup.CNPJ,
                Video = startup.Video,
                NomeStartup = startup.NomeStartup,
                Site = startup.Site,
                Descricao = startup.Descricao,
                NomeResponsavel = startup.NomeResponsavel,
                EmailStartup = startup.EmailStartup
            };
        }

        /// <summary>
        /// Retorna todos os Startup.
        /// </summary>
        public async Task<List<CreateStartupResponse>> GetAllPagedAsync()
        {
            var startup = await _repository.GetAllAsync();

            return startup.Select(u => new CreateStartupResponse
            {
                CNPJ = u.CNPJ,
                NomeStartup = u.NomeStartup,
                Video = u.Video,
                EmailStartup = u.EmailStartup
            }).ToList();
        }


        public async Task<CreateStartupResponse?> GetByIdAsync(string cnpj)
        {
            var startup = await _context.Startup
                .Include(u => u.Avaliacoes)
                .FirstOrDefaultAsync(u => u.CNPJ == cnpj);

            return new CreateStartupResponse
            {
                CNPJ = startup.CNPJ,
                Video = startup.Video,
                NomeStartup = startup.NomeStartup,
                Site = startup.Site,
                Descricao = startup.Descricao,
                NomeResponsavel = startup.NomeResponsavel,
                EmailStartup = startup.EmailStartup,
                UsuarioCPF = startup.UsuarioCPF,

                Avaliacoes = startup.Avaliacoes.Select(s => new CreateAvaliacaoResponse
                {
                    IdAvaliacao = s.IdAvaliacao,
                    Nota = s.Nota,
                    Comentario = s.Comentario,
                    UsuarioCPF = s.UsuarioCPF
                }).ToList()
            };
        }


        public async Task<bool> UpdateStartupAsync(string cnpj, CreateStartupRequest request)
        {
            var startup = await _repository.GetByIdAsync(cnpj);
            if (startup == null) return false;

            startup.Atualizar(
                request.CNPJ,
                request.Video,
                request.NomeStartup,
                request.Site,
                request.Descricao,
                request.NomeResponsavel,
                request.EmailStartup,
                request.UsuarioCPF
            );
            _repository.Update(startup);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStartupAsync(string cnpj)
        {
            var startup = await _repository.GetByIdAsync(cnpj);
            if (startup == null) return false;

            _repository.Delete(startup);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
