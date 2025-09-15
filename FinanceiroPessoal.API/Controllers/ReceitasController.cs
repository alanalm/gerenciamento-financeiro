using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Aplicacao.Servicos;
using FinanceiroPessoal.Dominio.Comum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceiroPessoal.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReceitasController : ControllerBase
    {
        private readonly IReceitaServico _receitaServico;

        public ReceitasController(IReceitaServico receitaServico)
        {
            _receitaServico = receitaServico;
        }

        // Obtém o ID do usuário autenticado a partir dos claims
        private Guid GetUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Usuário não autenticado ou claim de identificação ausente.");

            return Guid.Parse(userIdClaim);
        }

        [HttpPost]
        public async Task<ActionResult<RespostaApi<ReceitaDto>>> Post([FromBody] CriarReceitaDto receitaDto)
        {
            var result = await _receitaServico.AdicionarReceita(receitaDto, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RespostaApi<ReceitaDto>>> Get(string id)
        {
            var result = await _receitaServico.ObterReceitaPorId(id, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet]
        public async Task<ActionResult<RespostaApi<IEnumerable<ReceitaDto>>>> GetPorUsuario()
        {
            var result = await _receitaServico.ObterReceitasPorUsuario(GetUsuarioId());
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RespostaApi<ReceitaDto>>> Put(string id, [FromBody] AtualizarReceitaDto receitaDto)
        {
            var result = await _receitaServico.AtualizarReceita(id, receitaDto, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<RespostaApi<bool>>> Delete(string id)
        {
            var result = await _receitaServico.RemoverReceita(id, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}
