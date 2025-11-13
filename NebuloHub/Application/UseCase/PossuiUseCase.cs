using Microsoft.EntityFrameworkCore;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;

namespace NebuloHub.Application.UseCase
{
    public class PossuiUseCase
    {
        private readonly IRepository<Possui> _repository;
        private readonly AppDbContext _context;


        public PossuiUseCase(IRepository<Possui> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<CreatePossuiResponse> CreatePossuiAsync(CreatePossuiRequest request)
        {
            var possui = Possui.Create(
                request.StartupCNPJ,
                request.IdHabilidade
            );

            await _repository.AddAsync(possui);
            await _repository.SaveChangesAsync();

            return new CreatePossuiResponse
            {
                IdPossui = possui.IdPossui,
            };
        }

        /// <summary>
        /// Retorna os Possui paginados.
        /// </summary>
        public async Task<List<CreatePossuiResponse>> GetAllPagedAsync(int page, int pageSize)
        {
            var possui = await _repository.GetAllAsync();

            var paged = possui
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new CreatePossuiResponse
                {
                    IdPossui = u.IdPossui
                })
                .ToList();

            return paged;
        }

        public async Task<CreatePossuiResponse?> GetByIdAsync(long id)
        {
            var possui = await _context.Possui
                .Include(m => m.Startup)
                .Include(m => m.Habilidade)
                .FirstOrDefaultAsync(m => m.IdPossui == id);

            if (possui == null) return null;

            return new CreatePossuiResponse
            {
                IdPossui = possui.IdPossui,

                Startup = possui.Startup == null ? null : new CreateStartupResponse
                {
                    CNPJ = possui.Startup.CNPJ,
                    Video = possui.Startup.Video,
                    NomeStartup = possui.Startup.NomeStartup,
                    Site = possui.Startup.Site,
                    Descricao = possui.Startup.Descricao,
                    NomeResponsavel = possui.Startup.NomeResponsavel
                },
                Habilidade = possui.Habilidade == null ? null : new CreateHabilidadeResponse
                {
                    IdHabilidade = possui.Habilidade.IdHabilidade,
                    NomeHabilidade = possui.Habilidade.NomeHabilidade,
                    TipoHabilidade = possui.Habilidade.TipoHabilidade
                }
            };
        }


        public async Task<bool> UpdatePossuiAsync(long id, CreatePossuiRequest request)
        {
            var possui = await _repository.GetByIdAsync(id);
            if (possui == null) return false;

            possui.Atualizar(
                request.StartupCNPJ,
                request.IdHabilidade
            );
            _repository.Update(possui);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePossuiAsync(long id)
        {
            var possui = await _repository.GetByIdAsync(id);
            if (possui == null) return false;

            _repository.Delete(possui);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
