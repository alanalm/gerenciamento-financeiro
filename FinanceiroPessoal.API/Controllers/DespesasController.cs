using FinanceiroPessoal.Aplicacao.DTOs;
using FinanceiroPessoal.Aplicacao.Interfaces;
using FinanceiroPessoal.Aplicacao.Servicos;
using FinanceiroPessoal.Dominio.Comum;
using Microsoft.AspNetCore.Mvc;

namespace FinanceiroPessoal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DespesasController : ControllerBase
    {
        private readonly IDespesaServico _despesaServico;
        private readonly Guid usuarioTeste = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public DespesasController(IDespesaServico despesaServico)
        {
            _despesaServico = despesaServico;
        }

        // Simulação de UsuarioId, em um cenário real pegar do JWT
        private Guid GetUsuarioId() => usuarioTeste;
        //var usuarioId = User.Claims.First(c => c.Type == "sub").Value;

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
