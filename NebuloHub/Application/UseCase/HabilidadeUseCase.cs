using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Repositores;

namespace NebuloHub.Application.UseCase
{
    public class HabilidadeUseCase
    {
        private readonly IRepository<Habilidade> _repository;

        public HabilidadeUseCase(IRepository<Habilidade> repository)
        {
            _repository = repository;
        }

        public async Task<CreateHabilidadeResponse> CreateHabilidadeAsync(CreateHabilidadeRequest request)
        {
            var habilidade = Habilidade.Create(
                request.NomeHabilidade,
                request.TipoHabilidade
            );

            await _repository.AddAsync(habilidade);
            await _repository.SaveChangesAsync();

            return new CreateHabilidadeResponse
            {
                IdHabilidade = habilidade.IdHabilidade,
                NomeHabilidade = habilidade.NomeHabilidade,
                TipoHabilidade = habilidade.TipoHabilidade
            };
        }

        public async Task<List<CreateHabilidadeResponse>> GetAllHabilidadeAsync()
        {
            var habilidade = await _repository.GetAllAsync();
            return habilidade.Select(u => new CreateHabilidadeResponse
            {
                IdHabilidade = u.IdHabilidade,
                NomeHabilidade = u.NomeHabilidade,
                TipoHabilidade = u.TipoHabilidade
            }).ToList();
        }

        /// <summary>
        /// Retorna os Habilidade paginados.
        /// </summary>
        public async Task<List<CreateHabilidadeResponse>> GetAllPagedAsync(int page, int pageSize)
        {
            var habilidade = await _repository.GetAllAsync();

            var paged = habilidade
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new CreateHabilidadeResponse
                {
                    IdHabilidade = u.IdHabilidade,
                    NomeHabilidade = u.NomeHabilidade,
                    TipoHabilidade = u.TipoHabilidade
                })
                .ToList();

            return paged;
        }

        public async Task<CreateHabilidadeResponse?> GetByIdAsync(long id)
        {
            var habilidade = await _repository.GetByIdAsync(id);
            if (habilidade == null) return null;

            return new CreateHabilidadeResponse
            {
                IdHabilidade = habilidade.IdHabilidade,
                NomeHabilidade = habilidade.NomeHabilidade,
                TipoHabilidade = habilidade.TipoHabilidade
            };
        }

        public async Task<bool> UpdateHabilidadeAsync(long id, CreateHabilidadeRequest request)
        {
            var habilidade = await _repository.GetByIdAsync(id);
            if (habilidade == null) return false;

            habilidade.Atualizar(
                request.NomeHabilidade,
                request.TipoHabilidade
            );
            _repository.Update(habilidade);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHabilidadeAsync(long id)
        {
            var habilidade = await _repository.GetByIdAsync(id);
            if (habilidade == null) return false;

            _repository.Delete(habilidade);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}

