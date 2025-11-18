using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Tags("CRUD Avaliacao")]
    public class AvaliacaoController : ControllerBase
    {
        private readonly AvaliacaoUseCase _avaliacaoUseCase;
        private readonly CreateAvaliacaoRequestValidator _validationAvaliacao;

        // ILogger
        private readonly ILogger<AvaliacaoController> _logger;

        public AvaliacaoController(AvaliacaoUseCase avaliacaoUseCase, CreateAvaliacaoRequestValidator validationAvaliacao, ILogger<AvaliacaoController> logger)
        {
            _avaliacaoUseCase = avaliacaoUseCase;
            _validationAvaliacao = validationAvaliacao;
            _logger = logger;
        }




        /// <summary>
        /// Retorna todos os Avaliacao com paginação.
        /// </summary>
        /// <param name="page">Número da página (default = 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (default = 10)</param>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<CreateAvaliacaoResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAvaliacao([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            try
            {

                _logger.LogInformation("Iniciando busca de todas as avaliacoes...");

                var avaliacao = await _avaliacaoUseCase.GetAllPagedAsync(page, pageSize);

                _logger.LogInformation("Busca de avaliacao concluída. {count} registros encontrados.", avaliacao.Count());

                var result = avaliacao.Select(d => new
                {
                    d.IdAvaliacao,
                    d.Nota,
                    d.Comentario,
                    links = new
                    {
                        self = Url.Action(nameof(GetAvaliacaoById), new { id = d.IdAvaliacao })
                    }
                });

                return Ok(new
                {
                    page,
                    pageSize,
                    totalItems = avaliacao.Count(),
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
        /// Retorna um Avaliacao pelo ID.
        /// </summary>
        /// <param name="id">ID do registro</param>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateAvaliacaoResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAvaliacaoById(long id)
        {

            try
            {
                _logger.LogInformation("Buscando avaliacao com id {id)}", id);

                var avaliacao = await _avaliacaoUseCase.GetByIdAsync(id);
                if (avaliacao == null)
                {
                    _logger.LogWarning("Avaliacao {id} não encontrado.", id);
                    return NotFound();
                }

                _logger.LogInformation("Avaliacao {id} encontrado com sucesso.", id);
                return Ok(avaliacao);
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
        /// Cria um novo Avaliacao.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateAvaliacaoResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostAvaliacao([FromBody] CreateAvaliacaoRequest request)
        {

            try
            {
                _validationAvaliacao.ValidateAndThrow(request);

                var avaliacaoResponse = await _avaliacaoUseCase.CreateAvaliacaoAsync(request);
                return CreatedAtAction(nameof(GetAvaliacaoById), new { id = avaliacaoResponse.IdAvaliacao }, avaliacaoResponse);

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
        /// Atualiza um Avaliacao existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <param name="request">Payload para atualização</param>
        [HttpPut("{id}")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutAvaliacao(long id, [FromBody] CreateAvaliacaoRequest request)
        {

            try
            {
                _logger.LogInformation("Atualizando aavaliacao {id}", id);

                var updated = await _avaliacaoUseCase.UpdateAvaliacaoAsync(id, request);
                if (!updated)
                {
                    _logger.LogWarning("Tentativa de atualizar aavaliacao {id}, mas o registro não existe.", id);
                    return NotFound();
                }

                _logger.LogInformation("Avaliacao {id} atualizado com sucesso.", id);
                return Ok(new { mensagem = "Avaliacao atualizada com sucesso." });
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
        /// Deleta um Avaliacao existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAvaliacao(long id)
        {

            try
            {

                _logger.LogInformation("Deletando avaliacao {id}", id);

                var deleted = await _avaliacaoUseCase.DeleteAvaliacaoAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning("Tentativa de deletar avaliacao {id}, porém não encontrado.", id);
                    return NotFound();
                }

                _logger.LogInformation("Avaliacao {id} deletada com sucesso.", id);
                return Ok(new { mensagem = "Avaliacao deletada com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar a avaliacao.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }
    }
}