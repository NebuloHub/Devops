using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        // ILogger
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(
            UsuarioUseCase usuarioUseCase,
            CreateUsuarioRequestValidator validationUsuario,
            ILogger<UsuarioController> logger)
        {
            _usuarioUseCase = usuarioUseCase;
            _validationUsuario = validationUsuario;
            _logger = logger;
        }





        /// <summary>
        /// Retorna todos os Usuarios.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<CreateUsuarioResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsuario()
        {
            _logger.LogInformation("Iniciando busca de todos os usuários...");

            var usuario = await _usuarioUseCase.GetAllPagedAsync();

            _logger.LogInformation("Busca de usuários concluída. {count} registros encontrados.", usuario.Count());

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
                totalItems = usuario.Count(),
                items = result
            });
        }






        /// <summary>
        /// Retorna um Usuario pelo CPF.
        /// </summary>
        /// <param name="cpf">cpf do registro</param>
        [HttpGet("{cpf}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateUsuarioResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUsuarioById(string cpf)
        {
            _logger.LogInformation("Buscando usuário com CPF {cpf}", cpf);

            var usuario = await _usuarioUseCase.GetByIdAsync(cpf);
            if (usuario == null)
            {
                _logger.LogWarning("Usuário {cpf} não encontrado.", cpf);
                return NotFound();
            }

            _logger.LogInformation("Usuário {cpf} encontrado com sucesso.", cpf);
            return Ok(usuario);
        }








        /// <summary>
        /// Cria um novo Usuario.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CreateUsuarioResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostUsuario([FromBody] CreateUsuarioRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando criação do usuário {cpf}", request.CPF);

                // Valida entrada
                _validationUsuario.ValidateAndThrow(request);

                // Regra de negócio + persistência
                var usuarioResponse = await _usuarioUseCase.CreateUsuarioAsync(request);

                _logger.LogInformation("Usuário {cpf} criado com sucesso.", usuarioResponse.CPF);

                return CreatedAtAction(nameof(GetUsuarioById),
                    new { cpf = usuarioResponse.CPF }, usuarioResponse);
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
        /// Atualiza um Usuario existente.
        /// </summary>
        /// <param name="cpf">CPF do registro</param>
        /// <param name="request">Payload para atualização</param>
        [HttpPut("{cpf}")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutUsuario(string cpf, [FromBody] CreateUsuarioRequest request)
        {
            try
            {
                _logger.LogInformation("Atualizando usuário {cpf}", cpf);

                var atualizado = await _usuarioUseCase.UpdateUsuarioAsync(cpf, request);

                if (atualizado == null)
                    return NotFound();

                return Ok(atualizado);
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
                _logger.LogError(ex, "Erro inesperado ao atualizar usuário.");
                return StatusCode(500, new { erro = ex.Message });
            }
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
            try
            {
                var deleted = await _usuarioUseCase.DeleteUsuarioAsync(cpf);

                if (!deleted)
                    return NotFound();

                return Ok(new { mensagem = "Usuário deletado com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar usuário.");
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }
}
