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
    [Tags("CRUD Possui")]
    public class PossuiController : ControllerBase
    {
        private readonly PossuiUseCase _possuiUseCase;
        private readonly CreatePossuiRequestValidator _validationPossui;

        public PossuiController(PossuiUseCase possuiUseCase, CreatePossuiRequestValidator validationPossui)
        {
            _possuiUseCase = possuiUseCase;
            _validationPossui = validationPossui;
        }

        /// <summary>
        /// Retorna todos os Possui com paginação.
        /// </summary>
        /// <param name="page">Número da página (default = 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (default = 10)</param>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<CreatePossuiResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPossui([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var possui = await _possuiUseCase.GetAllPagedAsync(page, pageSize);

            var result = possui.Select(d => new
            {
                d.IdPossui,
                links = new
                {
                    self = Url.Action(nameof(GetPossuiById), new { id = d.IdPossui })
                }
            });

            return Ok(new
            {
                page,
                pageSize,
                totalItems = possui.Count(),
                items = result
            });
        }

        /// <summary>
        /// Retorna um Possui pelo ID.
        /// </summary>
        /// <param name="id">ID do registro</param>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreatePossuiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPossuiById(long id)
        {
            var possui = await _possuiUseCase.GetByIdAsync(id);
            if (possui == null)
                return NotFound();


            return Ok(possui);
        }

        /// <summary>
        /// Cria um novo Possui.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(CreatePossuiResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostPossui([FromBody] CreatePossuiRequest request)
        {
            _validationPossui.ValidateAndThrow(request);

            var possuiResponse = await _possuiUseCase.CreatePossuiAsync(request);
            return CreatedAtAction(nameof(GetPossuiById), new { id = possuiResponse.IdPossui }, possuiResponse);
        }

        /// <summary>
        /// Atualiza um Possui existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <param name="request">Payload para atualização</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutPossui(long id, [FromBody] CreatePossuiRequest request)
        {
            var updated = await _possuiUseCase.UpdatePossuiAsync(id, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deleta um Possui existente.
        /// </summary>
        /// <param name="id">ID do registro</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeletePossui(long id)
        {
            var deleted = await _possuiUseCase.DeletePossuiAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}