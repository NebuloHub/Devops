using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NebuloHub.Application.UseCase;
using NebuloHub.Services;
using System.Security.Claims;

namespace NebuloHub.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UsuarioUseCase _usuarioUseCase;

        public AuthController(TokenService tokenService, UsuarioUseCase usuarioUseCase)
        {
            _tokenService = tokenService;
            _usuarioUseCase = usuarioUseCase;
        }

        /// <summary>
        /// Endpoint de login (gera o token JWT)
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _usuarioUseCase.LoginAsync(request.Email, request.Senha);

            if (usuario == null)
                return Unauthorized("Usuário ou senha inválidos");

            var token = _tokenService.GenerateToken(usuario.Email, usuario.Role.ToString());

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

        /// <summary>
        /// Endpoint protegido com JWT
        /// </summary>
        [HttpGet("dados-protegidos")]
        [Authorize]
        public IActionResult GetDadosProtegidos()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok($"Bem-vindo, {username}! Seu papel é: {role}. Você acessou um endpoint protegido");
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }
    }
}
