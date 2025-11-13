using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.DTOs.Response;
using NebuloHub.Application.UseCase;
using NebuloHub.Application.Validators;
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

        public AvaliacaoController(AvaliacaoUseCase avaliacaoUseCase, CreateAvaliacaoRequestValidator validationAvaliacao)
        {
            _avaliacaoUseCase = avaliacaoUseCase;
            _validationAvaliacao = validationAvaliacao;
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
            var avaliacao = await _avaliacaoUseCase.GetAllPagedAsync(page, pageSize);

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
            var avaliacao = await _avaliacaoUseCase.GetByIdAsync(id);
            if (avaliacao == null)
                return NotFound();


            return Ok(avaliacao);
        }

        /// <summary>
        /// Cria um novo Avaliacao.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(CreateAvaliacaoResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostAvaliacao([FromBody] CreateAvaliacaoRequest request)
        {
            _validationAvaliacao.ValidateAndThrow(request);

            var avaliacaoResponse = await _avaliacaoUseCase.CreateAvaliacaoAsync(request);
            return CreatedAtAction(nameof(GetAvaliacaoById), new { id = avaliacaoResponse.IdAvaliacao }, avaliacaoResponse);
        }

        /// <summary>
        /// Atualiza um Avaliacao existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <param name="request">Payload para atualização</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutAvaliacao(long id, [FromBody] CreateAvaliacaoRequest request)
        {
            var updated = await _avaliacaoUseCase.UpdateAvaliacaoAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
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
            var deleted = await _avaliacaoUseCase.DeleteAvaliacaoAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}