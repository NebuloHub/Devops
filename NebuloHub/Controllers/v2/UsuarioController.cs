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
    [Tags("CRUD Usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioUseCase _usuarioUseCase;
        private readonly CreateUsuarioRequestValidator _validationUsuario;

        public UsuarioController(UsuarioUseCase usuarioUseCase, CreateUsuarioRequestValidator validationUsuario)
        {
            _usuarioUseCase = usuarioUseCase;
            _validationUsuario = validationUsuario;
        }

        /// <summary>
        /// Retorna todos os Usuario com paginação.
        /// </summary>
        /// <param name="page">Número da página (default = 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (default = 10)</param>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<CreateUsuarioResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsuario([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var usuario = await _usuarioUseCase.GetAllPagedAsync(page, pageSize);

            var result = usuario.Select(d => new
            {
                d.CPF,
                d.Nome,
                d.Email,
                links = new
                {
                    self = Url.Action(nameof(GetUsuarioById), new { cpf = d.CPF })
                }
            });

            return Ok(new
            {
                page,
                pageSize,
                totalItems = usuario.Count(),
                items = result
            });
        }

        /// <summary>
        /// Retorna um Usuario pelo CNPJ.
        /// </summary>
        /// <param name="cpf">CPF do registro</param>
        [HttpGet("{cpf}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateUsuarioResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUsuarioById(string cpf)
        {
            var usuario = await _usuarioUseCase.GetByIdAsync(cpf);
            if (usuario == null)
                return NotFound();


            return Ok(usuario);
        }

        /// <summary>
        /// Cria um novo Usuario.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(CreateUsuarioResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostUsuario([FromBody] CreateUsuarioRequest request)
        {
            _validationUsuario.ValidateAndThrow(request);

            var usuarioResponse = await _usuarioUseCase.CreateUsuarioAsync(request);
            return CreatedAtAction(nameof(GetUsuarioById), new { cpf = usuarioResponse.CPF }, usuarioResponse);
        }

        /// <summary>
        /// Atualiza um Usuario existente.
        /// </summary>
        /// <param name="cpf">CPF do registro</param>
        /// <param name="request">Payload para atualização</param>
        [HttpPut("{cpf}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutUsuario(string cpf, [FromBody] CreateUsuarioRequest request)
        {
            var updated = await _usuarioUseCase.UpdateUsuarioAsync(cpf, request);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Deleta um Usuario existente.
        /// </summary>
        /// <param name="cpf">ID do registro</param>
        [HttpDelete("{cpf}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteUsuario(string cpf)
        {
            var deleted = await _usuarioUseCase.DeleteUsuarioAsync(cpf);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}