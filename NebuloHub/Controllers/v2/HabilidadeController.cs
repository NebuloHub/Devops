using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Application.UseCase;
using NebuloHub.Application.Validators;
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

        public HabilidadeController(HabilidadeUseCase habilidadeUseCase, CreateHabilidadeRequestValidator validationHabilidade)
        {
            _habilidadeUseCase = habilidadeUseCase;
            _validationHabilidade = validationHabilidade;
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
            var habilidade = await _habilidadeUseCase.GetAllPagedAsync(page, pageSize);

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
            var habilidade = await _habilidadeUseCase.GetByIdAsync(id);
            if (habilidade == null)
                return NotFound();


            return Ok(habilidade);
        }

        /// <summary>
        /// Cria um novo Habilidade.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(CreateHabilidadeResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostHabilidade([FromBody] CreateHabilidadeRequest request)
        {
            _validationHabilidade.ValidateAndThrow(request);

            var habilidadeResponse = await _habilidadeUseCase.CreateHabilidadeAsync(request);
            return CreatedAtAction(nameof(GetHabilidadeById), new { id = habilidadeResponse.IdHabilidade }, habilidadeResponse);
        }

        /// <summary>
        /// Atualiza um habilidade existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <param name="request">Payload para atualização</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutHabilidade(long id, [FromBody] CreateHabilidadeRequest request)
        {
            var updated = await _habilidadeUseCase.UpdateHabilidadeAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
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
            var deleted = await _habilidadeUseCase.DeleteHabilidadeAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}