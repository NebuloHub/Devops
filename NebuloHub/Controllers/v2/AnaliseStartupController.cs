using Microsoft.AspNetCore.Mvc;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.UseCase;

namespace NebuloHub.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Tags("Analise das Startups")]
    public class AnaliseStartupController : ControllerBase
    {
        private readonly AnaliseStartupUseCase _useCase;

        public AnaliseStartupController(AnaliseStartupUseCase useCase)
        {
            _useCase = useCase;
        }

        /// <summary>
        /// Chama a procedure pkg_funcao2_validacao.analisar_startup e retorna o resultado do CLOB.
        /// </summary>
        [HttpPost("analisar")]
        public async Task<IActionResult> AnalisarStartup([FromBody] AnalisarStartupRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CNPJ))
                return BadRequest("CNPJ é obrigatório.");

            var result = await _useCase.AnalisarAsync(request);
            return Ok(result);
        }
    }
}