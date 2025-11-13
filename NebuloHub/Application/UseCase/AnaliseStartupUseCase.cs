using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Infraestructure.Repositores;
using NebuloHub.Infraestructure.Repositories;

namespace NebuloHub.Application.UseCase
{
    public class AnaliseStartupUseCase
    {
        private readonly StartupProcedureRepository _procedureRepository;

        public AnaliseStartupUseCase(StartupProcedureRepository procedureRepository)
        {
            _procedureRepository = procedureRepository;
        }

        public async Task<AnalisarStartupResponse> AnalisarAsync(AnalisarStartupRequest request)
        {
            var resultado = await _procedureRepository.AnalisarStartupAsync(request.CNPJ);

            return new AnalisarStartupResponse
            {
                Resultado = resultado
            };
        }
    }
}
