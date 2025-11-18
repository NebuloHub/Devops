using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NebuloHub.Application.DTOs.Request;
using NebuloHub.Application.UseCase;
using NebuloHub.Services;
using System.Security.Claims;

namespace NebuloHub.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UsuarioUseCase _usuarioUseCase;

        private readonly ILogger<AuthController> _logger;

        public AuthController(TokenService tokenService, UsuarioUseCase usuarioUseCase, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _usuarioUseCase = usuarioUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint de login (gera o token JWT)
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            try
            {
                _logger.LogInformation("Iniciando login do usuario...");

                var usuario = await _usuarioUseCase.LoginAsync(request.Email, request.Senha);

                if (usuario == null)
                    return Unauthorized("Usuário ou senha inválidos");

                var token = _tokenService.GenerateToken(usuario.CPF, usuario.Email, usuario.Role.ToString());

                return Ok(new
                {
                    token,
                    usuario = new
                    {
                        usuario.Nome,
                        usuario.Email,
                        usuario.Role
                    }
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado.");
                return StatusCode(500, new { erro = ex.Message });
            }

        }

        /// <summary>
        /// Endpoint protegido com JWT
        /// </summary>
        [HttpGet("dados-protegidos")]
        [Authorize]
        public IActionResult GetDadosProtegidos()
        {

            try
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                return Ok($"Bem-vindo, {username}! Seu papel é: {role}. Você acessou um endpoint protegido");
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

    }
}
