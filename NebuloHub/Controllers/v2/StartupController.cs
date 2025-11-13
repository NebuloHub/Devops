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
    [Tags("CRUD Startup")]
    public class StartupController : ControllerBase
    {
        private readonly StartupUseCase _startupUseCase;
        private readonly CreateStartupRequestValidator _validationStartup;

        public StartupController(StartupUseCase startupUseCase, CreateStartupRequestValidator validationStartup)
        {
            _startupUseCase = startupUseCase;
            _validationStartup = validationStartup;
        }

        /// <summary>
        /// Retorna todos os Startup com paginação.
        /// </summary>
        /// <param name="page">Número da página (default = 1)</param>
        /// <param name="pageSize">Quantidade de itens por página (default = 10)</param>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<CreateStartupResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStartup([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var startup = await _startupUseCase.GetAllPagedAsync(page, pageSize);

            var result = startup.Select(d => new
            {
                d.CNPJ,
                d.NomeStartup,
                d.EmailStartup,
                links = new
                {
                    self = Url.Action(nameof(GetStartupById), new { cnpj = d.CNPJ })
                }
            });

            return Ok(new
            {
                page,
                pageSize,
                totalItems = startup.Count(),
                items = result
            });
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
            var startup = await _startupUseCase.GetByIdAsync(cnpj);
            if (startup == null)
                return NotFound();


            return Ok(startup);
        }

        /// <summary>
        /// Cria um novo Startup.
        /// </summary>
        /// <param name="request">Payload para criação</param>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(CreateStartupResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostStartup([FromBody] CreateStartupRequest request)
        {
            _validationStartup.ValidateAndThrow(request);

            var startupResponse = await _startupUseCase.CreateStartupAsync(request);
            return CreatedAtAction(nameof(GetStartupById), new { cnpj = startupResponse.CNPJ }, startupResponse);
        }

        /// <summary>
        /// Atualiza um Startup existente.
        /// </summary>
        /// <param name="cnpj">CNPJ do registro</param>
        /// <param name="request">Payload para atualização</param>
        [HttpPut("{cnpj}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutStartup(string cnpj, [FromBody] CreateStartupRequest request)
        {
            var updated = await _startupUseCase.UpdateStartupAsync(cnpj, request);
            if (!updated)
                return NotFound();

            return NoContent();
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
            var deleted = await _startupUseCase.DeleteStartupAsync(cnpj);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
