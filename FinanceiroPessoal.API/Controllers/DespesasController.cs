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
    public class DespesasController : ControllerBase
    {
        private readonly IDespesaServico _despesaServico;

        public DespesasController(IDespesaServico despesaServico)
        {
            _despesaServico = despesaServico;
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
        public async Task<ActionResult<RespostaApi<DespesaDto>>> Post([FromBody] CriarDespesaDto despesaDto)
        {
            var result = await _despesaServico.AdicionarDespesa(despesaDto, GetUsuarioId());
            if (result.Sucesso)
            {
                return CreatedAtAction(nameof(Get), new { id = result.Dados.Id }, result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RespostaApi<DespesaDto>>> Get(string id)
        {
            var result = await _despesaServico.ObterDespesaPorId(id, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        [HttpGet]
        public async Task<ActionResult<RespostaApi<IEnumerable<DespesaDto>>>> GetPorUsuario()
        {
            var result = await _despesaServico.ObterDespesasPorUsuario(GetUsuarioId());
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RespostaApi<DespesaDto>>> Put(string id, [FromBody] AtualizarDespesaDto despesaDto)
        {
            var result = await _despesaServico.AtualizarDespesa(id, despesaDto, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<RespostaApi<bool>>> Delete(string id)
        {
            var result = await _despesaServico.RemoverDespesa(id, GetUsuarioId());
            if (result.Sucesso)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}
