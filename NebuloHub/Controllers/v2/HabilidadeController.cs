using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Application.UseCase;
using NebuloHub.Application.Validators;
using NebuloHub.Domain.Entity;
using System;
using System.Net;

namespace NebuloHub.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Tags("CRUD Habilidade")]
    public class HabilidadeController : ControllerBase
    {
        private readonly HabilidadeUseCase _habilidadeUseCase;
        private readonly CreateHabilidadeRequestValidator _validationHabilidade;

        private readonly ILogger<HabilidadeController> _logger;

        public HabilidadeController(HabilidadeUseCase habilidadeUseCase, CreateHabilidadeRequestValidator validationHabilidade, ILogger<HabilidadeController> logger)
        {
            _habilidadeUseCase = habilidadeUseCase;
            _validationHabilidade = validationHabilidade;
            _logger = logger;
        }



        /// <summary>
        /// Retorna todos os Habilidade com paginação.
        /// </summary>
        /// <param name="page">Número da página (default = 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (default = 10)</param>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<CreateHabilidadeResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHabilidade([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {

            try
            {

                _logger.LogInformation("Iniciando busca de todas as habilidades...");

                var habilidade = await _habilidadeUseCase.GetAllPagedAsync(page, pageSize);

                _logger.LogInformation("Busca de habilidade concluída. {count} registros encontrados.", habilidade.Count());

                var result = habilidade.Select(d => new
                {
                    d.IdHabilidade,
                    d.NomeHabilidade,
                    d.TipoHabilidade,
                    links = new
                    {
                        self = Url.Action(nameof(GetHabilidadeById), new { id = d.IdHabilidade })
                    }
                });

                return Ok(new
                {
                    page,
                    pageSize,
                    totalItems = habilidade.Count(),
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
        /// Retorna um Habilidade pelo ID.
        /// </summary>
        /// <param name="id">ID do registro</param>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateHabilidadeResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetHabilidadeById(long id)
        {

            try
            {

                _logger.LogInformation("Buscando habilidade com id {id)}", id);

                var habilidade = await _habilidadeUseCase.GetByIdAsync(id);
                if (habilidade == null)
                {
                    _logger.LogWarning("Habilidade {id} não encontrado.", id);
                    return NotFound();
                }
                    

                return Ok(habilidade);
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
        /// Cria um novo Habilidade.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateHabilidadeResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostHabilidade([FromBody] CreateHabilidadeRequest request)
        {

            try
            {
                _validationHabilidade.ValidateAndThrow(request);

                var habilidadeResponse = await _habilidadeUseCase.CreateHabilidadeAsync(request);
                return CreatedAtAction(nameof(GetHabilidadeById), new { id = habilidadeResponse.IdHabilidade }, habilidadeResponse);
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
        /// Atualiza um habilidade existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <param name="request">Payload para atualização</param>
        [HttpPut("{id}")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutHabilidade(long id, [FromBody] CreateHabilidadeRequest request)
        {

            try
            {

                _logger.LogInformation("Atualizando habilidade {id}", id);

                var updated = await _habilidadeUseCase.UpdateHabilidadeAsync(id, request);
                if (!updated)
                {
                    _logger.LogWarning("Tentativa de atualizar habilidade {id}, mas o registro não existe.", id);
                    return NotFound();
                }
                    

                _logger.LogInformation("Habilidade {id} atualizado com sucesso.", id);
                return NoContent();
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
                _logger.LogError(ex, "Erro inesperado ao atualizar habilidade.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }





        /// <summary>
        /// Deleta um Habilidade existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteHabilidade(long id)
        {

            try
            {
                _logger.LogInformation("Deletando habilidade {id}", id);

                var deleted = await _habilidadeUseCase.DeleteHabilidadeAsync(id);
                if (!deleted)
                    return NotFound();


                _logger.LogInformation("Habilidade {id} deletada com sucesso.", id);
                return Ok(new { mensagem = "Habilidade deletado com sucesso." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar a habilidade.");
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }
}