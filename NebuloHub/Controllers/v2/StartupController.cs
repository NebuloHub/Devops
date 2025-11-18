using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Application.UseCase;
using NebuloHub.Application.Validators;
using NebuloHub.Domain.Entity;
using System.Net;

namespace NebuloHub.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Tags("CRUD Startup")]
    public class StartupController : ControllerBase
    {
        private readonly StartupUseCase _startupUseCase;
        private readonly CreateStartupRequestValidator _validationStartup;

        // ILogger
        private readonly ILogger<StartupController> _logger;

        public StartupController(StartupUseCase startupUseCase, CreateStartupRequestValidator validationStartup, ILogger<StartupController> logger)
        {
            _startupUseCase = startupUseCase;
            _validationStartup = validationStartup;
            _logger = logger;
        }




        /// <summary>
        /// Retorna todos os Startup.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<CreateStartupResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStartup()
        {

            try
            {
                _logger.LogInformation("Iniciando busca de todas as startups...");

                var startup = await _startupUseCase.GetAllPagedAsync();

                _logger.LogInformation("Busca de startups concluída. {count} registros encontrados.", startup.Count());

                var result = startup.Select(d => new
                {
                    d.CNPJ,
                    d.NomeStartup,
                    d.Video,
                    d.EmailStartup,
                    links = new
                    {
                        self = Url.Action(nameof(GetStartupById), new { cnpj = d.CNPJ })
                    }
                });

                return Ok(new
                {
                    totalItems = startup.Count(),
                    items = result
                });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { erro = "Erro ao acessar o banco: " + ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }





        /// <summary>
        /// Retorna um Startup pelo CNPJ.
        /// </summary>
        /// <param name="cnpj">cnpj do registro</param>
        [HttpGet("{cnpj}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateStartupResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetStartupById(string cnpj)
        {

            try
            {
                _logger.LogInformation("Buscando startup com CNPJ {cnpj)}", cnpj);

                var startup = await _startupUseCase.GetByIdAsync(cnpj);
                if (startup == null)
                {
                    _logger.LogWarning("Startup {cnpj} não encontrado.", cnpj);
                    return NotFound();
                }

                _logger.LogInformation("Startup {cnpj} encontrado com sucesso.", cnpj);
                return Ok(startup);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { erro = "Erro ao acessar o banco: " + ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }




        /// <summary>
        /// Cria um novo Startup.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateStartupResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostStartup([FromBody] CreateStartupRequest request)
        {

            try
            {
                _logger.LogInformation("Iniciando criação do startup {CNPJ}", request.CNPJ);

                _validationStartup.ValidateAndThrow(request);

                var startupResponse = await _startupUseCase.CreateStartupAsync(request);

                _logger.LogInformation("Startup {CNPJ} criado com sucesso.", startupResponse.CNPJ);

                return CreatedAtAction(nameof(GetStartupById), new { cnpj = startupResponse.CNPJ }, startupResponse);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { erro = "Erro ao acessar o banco: " + ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no cadastro.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }





        /// <summary>
        /// Atualiza um Startup existente.
        /// </summary>
        /// <param name="cnpj">CNPJ do registro</param>
        /// <param name="request">Payload para atualização</param>
        [HttpPut("{cnpj}")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutStartup(string cnpj, [FromBody] CreateStartupRequest request)
        {

            try
            {
                _logger.LogInformation("Atualizando startup {cnpj}", cnpj);

                var updated = await _startupUseCase.UpdateStartupAsync(cnpj, request);
                if (!updated)
                {
                    _logger.LogWarning("Tentativa de atualizar startup {cnpj}, mas o registro não existe.", cnpj);
                    return NotFound();
                }

                _logger.LogInformation("Startup {cnpj} atualizado com sucesso.", cnpj);
                return Ok(new { mensagem = "Startup atualizada com sucesso." });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { erro = "Erro no banco: " + ex.InnerException?.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar startup.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }





        /// <summary>
        /// Deleta um Startup existente.
        /// </summary>
        /// <param name="cnpj">ID do registro</param>
        [HttpDelete("{cnpj}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteStartup(string cnpj)
        {

            try
            {
                _logger.LogInformation("Deletando startup {cnpj}", cnpj);

                var deleted = await _startupUseCase.DeleteStartupAsync(cnpj);
                if (!deleted)
                {
                    _logger.LogWarning("Tentativa de deletar startup {cnpj}, porém não encontrado.", cnpj);
                    return NotFound();
                }

                _logger.LogInformation("Startup {cnpj} deletada com sucesso.", cnpj);
                return Ok(new { mensagem = "Startup deletado com sucesso." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar a startup.");
                return StatusCode(500, new { erro = ex.Message });
            }
           
        }
    }
}
